using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using phoenix.core.Data;
using phoenix.core.Domain;
using phoenix.core.Http;
using phoenix.requests.Leviathan;

namespace phoenix.requests.Customers
{
  /// <summary>
  /// Syncs the new Customer to Leviathan if possible, with retry.
  /// On success: attempts to update the Customer with the LeviathanId.
  /// On failure: queues the request for later manual / automatic processing.
  /// </summary>
  public class SyncCustomerToLeviathan : LeviathanSyncNotificationHandler<CustomerCreatedEvent, Customer>
  {
    public SyncCustomerToLeviathan(ILeviathanClient leviathanClient,
      IDeadLetterQueueBroker<Customer> queue,
      ILogger<SyncCustomerToLeviathan> logger,
      IMongoDatabaseProvider mongoDatabaseProvider) : base(leviathanClient, queue, mongoDatabaseProvider, logger)
    {
    }

    public override async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
      await HandleCore(notification, "/customer", cancellationToken);
    }
  }
}
