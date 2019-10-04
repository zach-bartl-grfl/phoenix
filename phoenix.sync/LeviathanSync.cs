using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using phoenix.core.Data;
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
    private readonly IMongoDatabaseProvider _mongoDatabaseProvider;
    private readonly ILogger<LeviathanSync> _logger;
    private IEnumerable<string> _syncedCustomerIds = new List<string>();

    public LeviathanSync(IOptionsMonitor<LeviathanConfig> leviathanConfig,
      ILeviathanClient leviathanClient,
      IMediator mediator,
      IMongoDatabaseProvider mongoDatabaseProvider,
      ILogger<LeviathanSync> logger)
    {
      _leviathanConfig = leviathanConfig;
      _leviathanClient = leviathanClient;
      _mediator = mediator;
      _mongoDatabaseProvider = mongoDatabaseProvider;
      _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var customerSync = SyncCustomers(stoppingToken);
      var orderSync = SyncOrders(stoppingToken);
      await Task.WhenAll(customerSync, orderSync);
    }

    private async Task SyncCustomers(CancellationToken stoppingToken)
    {
      stoppingToken.Register(() =>
        _logger.LogError("Leviathan customer Sync background task is stopping."));

      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogError("Leviathan customer Sync background task is starting.");

        var customers = await _leviathanClient.GetAsync<IEnumerable<Customer>>("/customer/get-all",
          cancellationToken: stoppingToken);

        _syncedCustomerIds = customers?.Select(c => c.Id);

        foreach (var customer in customers ?? new List<Customer>())
        {
          _logger.LogError($"customer: {customer.Id} | {customer.Name}");
          customer.LeviathanId = customer.Id;
          await _mediator.Send(new CreateCustomerCommand {Customer = customer}, stoppingToken);
        }

        await Task.Delay(TimeSpan.FromSeconds(_leviathanConfig.CurrentValue.SyncDelay),
          stoppingToken);
      }

      _logger.LogError("Leviathan customer Sync background task is stopping at the request of the caller.");
    }

    private async Task SyncOrders(CancellationToken stoppingToken)
    {
      stoppingToken.Register(() =>
        _logger.LogError("Leviathan order Sync background task is stopping."));

      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogError("Leviathan order Sync background task is starting.");

        foreach (var customerId in _syncedCustomerIds)
        {
          var orders = await _leviathanClient.GetAsync<IEnumerable<Order>>($"/orders/{customerId}",
            cancellationToken: stoppingToken);

          foreach (var order in orders)
          {
            _logger.LogError($"order: {order.Id} | {order.CustomerId} | {order.CartTotal}");
            order.LeviathanId = order.Id;
            await _mediator.Send(new CreateOrderCommand {Order = order}, stoppingToken);
          }
        }

        await Task.Delay(TimeSpan.FromSeconds(_leviathanConfig.CurrentValue.SyncDelay),
          stoppingToken);
      }

      _logger.LogError("Leviathan order Sync background task is stopping at the request of the caller.");
      
    }
  }
}
