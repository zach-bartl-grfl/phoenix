using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using phoenix.core.Data;
using phoenix.core.Domain;

namespace phoenix.sync
{
  /// <summary>
  /// Retries failed Leviathan sync requests as appropriate.
  /// </summary>
  public class LeviathanRetry : BackgroundService
  {
    private readonly IDeadLetterQueueBroker<Customer> _customerDeadLetterQueue;

    public LeviathanRetry(IDeadLetterQueueBroker<Customer> customerDeadLetterQueue)
    {
      _customerDeadLetterQueue = customerDeadLetterQueue;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      // retry (with Polly?) missed customer syncs
      // retry (with Polly?) missed order syncs
      return Task.CompletedTask;
    }
  }
}
