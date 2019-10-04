using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using phoenix.core.Data;
using phoenix.core.Domain;
using phoenix.core.Exceptions;
using phoenix.core.Http;

namespace phoenix.requests.Leviathan
{
  public abstract class LeviathanSyncNotificationHandler<TNotification, TEntity> : INotificationHandler<TNotification>
    where TNotification : ILeviathanEntityNotification<TEntity>
    where TEntity : class, ILeviathanEntity
  {
    protected readonly ILeviathanClient LeviathanClient;
    protected readonly IDeadLetterQueueBroker<TEntity> Queue;
    protected readonly IMongoDatabaseProvider MongoDatabaseProvider;

    protected LeviathanSyncNotificationHandler(ILeviathanClient leviathanClient,
      IDeadLetterQueueBroker<TEntity> queue,
      IMongoDatabaseProvider mongoDatabaseProvider)
    {
      LeviathanClient = leviathanClient;
      Queue = queue;
      MongoDatabaseProvider = mongoDatabaseProvider;
    }
    
    public abstract Task Handle(TNotification notification, CancellationToken cancellationToken);

    protected async Task HandleCore(TNotification notification, string url, CancellationToken cancellationToken)
    {
      if (!string.IsNullOrEmpty(notification.Entity.LeviathanId)) return;

      TEntity entity;
      try
      {
        entity = await LeviathanClient.PostAsync(
          url,
          notification.Entity,
          cancellationToken);
      }
      catch (ThirdPartyServiceException ex)
      {
        // Publish unsuccessful Leviathan POST to Dead Letter Queue.
        // In a real environment this queue might exist in AMQP, Kafka, etc.
        // and we could register a service to reattempt previously unsuccessful
        // POST / PUT requests to Leviathan.
        Queue.Publish(new DeadLetter<TEntity>
        {
          Exception = ex.Message,
          Data = notification.Entity
        });

        throw;
      }
      
      if (entity?.Id != notification.Entity.Id)
      {
        var idFilter = Builders<TEntity>.Filter
          .Eq(c => c.Id, notification.Entity.Id);
        
        notification.Entity.LeviathanId = entity.Id;
        
        await MongoDatabaseProvider.Collection<TEntity>()
          .ReplaceOneAsync(idFilter, notification.Entity, cancellationToken: cancellationToken);
      }
    }
  }
}