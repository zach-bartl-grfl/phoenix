using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using phoenix.core.Data;
using phoenix.core.Domain;
using phoenix.core.Http;
using phoenix.requests.Leviathan;

namespace phoenix.requests.Orders
{
  /// <summary>
  /// Syncs the new Order to Leviathan if possible, with retry.
  /// On success: attempts to update the Order with the LeviathanId.
  /// On failure: queues the request for later manual / automatic processing.
  /// </summary>
  public class SyncOrderToLeviathan : LeviathanSyncNotificationHandler<OrderCreatedEvent, Order>
  {
    public SyncOrderToLeviathan(ILeviathanClient leviathanClient,
      IDeadLetterQueueBroker<Order> queue,
      ILogger<SyncOrderToLeviathan> logger,
      IMongoDatabaseProvider mongoDatabaseProvider) : base(leviathanClient, queue, mongoDatabaseProvider, logger)
    {
    }
    
    public override async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
      await HandleCore(notification, "/orders", cancellationToken);
    }
  }
}
