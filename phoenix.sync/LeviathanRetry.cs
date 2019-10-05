using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using phoenix.core.Data;
using phoenix.core.Domain;
using phoenix.requests.Customers;
using phoenix.requests.Orders;

namespace phoenix.sync
{
  /// <summary>
  /// Retries failed Leviathan sync requests as appropriate.
  /// TODO: make configurable, it might be preferable that dead letters are not retried
  /// </summary>
  public class LeviathanRetry : BackgroundService
  {
    private readonly IOptionsMonitor<LeviathanConfig> _leviathanConfig;
    private readonly IMediator _mediator;
    private readonly ILogger<LeviathanRetry> _logger;
    private readonly IDeadLetterQueueBroker<Customer> _customerDeadLetterQueue;
    private readonly IDeadLetterQueueBroker<Order> _orderDeadLetterQueue;

    public LeviathanRetry(IOptionsMonitor<LeviathanConfig> leviathanConfig,
      IMediator mediator,
      IDeadLetterQueueBroker<Customer> customerDeadLetterQueue,
      IDeadLetterQueueBroker<Order> orderDeadLetterQueue,
      ILogger<LeviathanRetry> logger)
    {
      _leviathanConfig = leviathanConfig;
      _mediator = mediator;
      _customerDeadLetterQueue = customerDeadLetterQueue;
      _orderDeadLetterQueue = orderDeadLetterQueue;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      try
      {
        var customerSync = RetryLeviathanEntitySync<Customer>(RetryCustomers, stoppingToken);
        var orderSync = RetryLeviathanEntitySync<Order>(RetryOrders, stoppingToken);
        await Task.WhenAll(customerSync, orderSync);
      }
      // see: https://github.com/aspnet/Extensions/issues/813
      catch (Exception ex)
      {
        _logger.LogError($"Error syncing with Leviathan: {ex}");
        throw;
      }
    }

    public async Task RetryCustomers(CancellationToken cancellationToken)
    {
      // retry (with Polly?) missed customer syncs
      while (_customerDeadLetterQueue.Any())
      {
        var customer = _customerDeadLetterQueue.Retrieve();
        await _mediator.Publish(new CustomerCreatedEvent {Entity = customer.Data}, cancellationToken);
      }
    }
    
    public async Task RetryOrders(CancellationToken cancellationToken)
    {
      // retry (with Polly?) missed order syncs
      while (_orderDeadLetterQueue.Any())
      {
        var customer = _orderDeadLetterQueue.Retrieve();
        await _mediator.Publish(new OrderCreatedEvent {Entity = customer.Data}, cancellationToken);
      }
    }

    public async Task RetryLeviathanEntitySync<T>(Func<CancellationToken, Task> syncf, CancellationToken stoppingToken)
    {
      stoppingToken.Register(() =>
        _logger.LogError($"Leviathan {typeof(T).Name} Retry background task is stopping."));

      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogInformation($"Leviathan {typeof(T).Name} Retry background task is starting.");

        await syncf(stoppingToken);

        await Task.Delay(TimeSpan.FromSeconds(_leviathanConfig.CurrentValue.RetryDelay > 0
            ? _leviathanConfig.CurrentValue.RetryDelay
            : 300),
          stoppingToken);
      }

      _logger.LogInformation($"Leviathan {typeof(T).Name} Retry background task is stopping at the request of the caller.");
    }
  }
}
