using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using phoenix.core.Domain;
using phoenix.core.Http;
using phoenix.requests.Customers;
using phoenix.requests.Orders;

namespace phoenix.sync
{
  /// <inheritdoc />
  /// <summary>
  /// Pulls and syncs new entities not present in Phoenix from the Leviathan Traceability API
  /// </summary>
  public class LeviathanSync : BackgroundService
  {
    private readonly IOptionsMonitor<LeviathanConfig> _leviathanConfig;
    private readonly ILeviathanClient _leviathanClient;
    private readonly IMediator _mediator;
    private readonly ILogger<LeviathanSync> _logger;
    private IEnumerable<string> _syncedCustomerIds = new List<string>();

    public LeviathanSync(IOptionsMonitor<LeviathanConfig> leviathanConfig,
      ILeviathanClient leviathanClient,
      IMediator mediator,
      ILogger<LeviathanSync> logger)
    {
      _leviathanConfig = leviathanConfig;
      _leviathanClient = leviathanClient;
      _mediator = mediator;
      _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      try
      {
        var customerSync = SyncLeviathanEntity<Customer>(SyncCustomers, stoppingToken);
        var orderSync = SyncLeviathanEntity<Order>(SyncOrders, stoppingToken);
        await Task.WhenAll(customerSync, orderSync);
      }
      // see: https://github.com/aspnet/Extensions/issues/813
      catch (Exception ex)
      {
        _logger.LogError($"Error syncing with Leviathan: {ex}");
        throw;
      }
    }

    private async Task SyncCustomers(CancellationToken stoppingToken)
    {
      var customers = await _leviathanClient.GetAsync<IEnumerable<Customer>>("/customer/get-all",
        cancellationToken: stoppingToken);

      _syncedCustomerIds = customers?.Select(c => c.Id);

      if (!customers.Any()) return;
        
      foreach (var customer in customers)
      {
        _logger.LogDebug($"customer: {customer.Id} | {customer.Name}");
        customer.LeviathanId = customer.Id;
        await _mediator.Send(new CreateCustomerCommand {Customer = customer}, stoppingToken);
      }
    }

    private async Task SyncOrders(CancellationToken stoppingToken)
    {
      if (!_syncedCustomerIds.Any()) return;
      
      foreach (var customerId in _syncedCustomerIds)
      {
        var orders = await _leviathanClient.GetAsync<IEnumerable<Order>>($"/orders/{customerId}",
          cancellationToken: stoppingToken);

        if (!orders.Any()) continue;
      
        foreach (var order in orders)
        {
          _logger.LogDebug($"order: {order.Id} | {order.CustomerId} | {order.CartTotal}");
          order.LeviathanId = order.Id;
          await _mediator.Send(new CreateOrderCommand {Order = order}, stoppingToken);
        }
      }
    }

    private async Task SyncLeviathanEntity<T>(Func<CancellationToken, Task> syncf, CancellationToken stoppingToken)
    {
      stoppingToken.Register(() =>
        _logger.LogError($"Leviathan {typeof(T).Name} Sync background task is stopping."));

      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogInformation($"Leviathan {typeof(T).Name} Sync background task is starting.");

        await syncf(stoppingToken);

        await Task.Delay(TimeSpan.FromSeconds(_leviathanConfig.CurrentValue.SyncDelay > 0
            ? _leviathanConfig.CurrentValue.SyncDelay
            : 300),
          stoppingToken);
      }

      _logger.LogInformation($"Leviathan {typeof(T).Name} Sync background task is stopping at the request of the caller.");
    }
  }
}
