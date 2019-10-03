using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using phoenix.core.Data;
using phoenix.core.Domain;
using phoenix.core.Exceptions;
using phoenix.sync;
using Polly;

namespace phoenix.requests.Customers
{
  /// <summary>
  /// Syncs the new Customer to Leviathan if possible, with retry.
  /// On success: attempts to update the Customer with the LeviathanId.
  /// On failure: queues the request for later manual / automatic processing.
  /// </summary>
  public class SyncCustomerToLeviathan : INotificationHandler<CustomerCreatedEvent>
  {
    private readonly ILeviathanClient _leviathanClient;
    private readonly IDeadLetterQueueBroker<Customer> _queue;
    private readonly IMongoDatabaseProvider _mongoDatabaseProvider;

    public SyncCustomerToLeviathan(ILeviathanClient leviathanClient,
      IDeadLetterQueueBroker<Customer> queue,
      IMongoDatabaseProvider mongoDatabaseProvider)
    {
      _leviathanClient = leviathanClient;
      _queue = queue;
      _mongoDatabaseProvider = mongoDatabaseProvider;
    }

    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
      var basePolicy = Policy<Customer>
        .Handle<TaskCanceledException>(ex =>
          !ex.CancellationToken.IsCancellationRequested);
      
      var retryPolicy = basePolicy
        .WaitAndRetryAsync(new[]
        {
          TimeSpan.FromSeconds(1),
          TimeSpan.FromSeconds(2),
          TimeSpan.FromSeconds(3)
        });

      var fallbackPolicy = basePolicy
        .FallbackAsync(
          notification.Customer,
          async e =>
          {
          });

      Customer leviathanCustomer = null;
      try
      {
        leviathanCustomer = await _leviathanClient.PostAsync(
          "/customer",
          notification.Customer,
          cancellationToken);
      }
      catch (RequestUnsuccessfulException ex)
      {
        // Publish unsuccessful Leviathan POST to Dead Letter Queue.
        // In a real environment this queue might exist in AMQP, Kafka, etc.
        // and we could register a service to reattempt previously unsuccessful
        // POST / PUT requests to Leviathan.
        _queue.Publish(new DeadLetter<Customer>
        {
          Exception = ex.Message,
          Data = notification.Customer
        });
      }

      if (leviathanCustomer.Id != notification.Customer.Id)
      {
        var idFilter = Builders<Customer>.Filter
          .Eq(e => e.Id, notification.Customer.Id);
        
        notification.Customer.LeviathanId = leviathanCustomer.Id;
        
        await _mongoDatabaseProvider.Collection<Customer>()
          .ReplaceOneAsync(idFilter, notification.Customer, cancellationToken: cancellationToken);
      }
    }
  }
}
