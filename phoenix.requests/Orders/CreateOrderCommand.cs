using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using phoenix.core.Data;
using phoenix.core.Domain;

namespace phoenix.requests.Orders
{
  /// <summary>
  /// Requests the creation of a new Phoenix Order
  /// </summary>
  public class CreateOrderCommand : IRequest
  {
    public Order Order { get; set; }
  }
  
  /// <summary>
  /// Persists the local Phoenix Order and notifies all listeners of a new Order event.
  /// </summary>
  public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Unit>
  {
    private readonly IMediator _mediator;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IMongoCollection<Customer> _customerCollection;
    private readonly IMongoCollection<Order> _orderCollection;

    public CreateOrderCommandHandler(IMongoDatabaseProvider mongoDatabaseProvider,
      IMediator mediator,
      ILogger<CreateOrderCommandHandler> logger)
    {
      _mediator = mediator;
      _logger = logger;
      _customerCollection = mongoDatabaseProvider.Collection<Customer>();
      _orderCollection = mongoDatabaseProvider.Collection<Order>();
    }
    
    public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
      try
      {
        await _orderCollection
          .InsertOneAsync(request.Order, cancellationToken: cancellationToken);
      }
      catch (MongoWriteException)
      {
        _logger.LogInformation($"Order already exists: {request.Order.Id} {request.Order.LeviathanId}");
        return Unit.Value;
      }
      
      var customer = (await _customerCollection.FindAsync(
        Builders<Customer>.Filter.Eq(c => c.Id, request.Order.CustomerId),
        cancellationToken: cancellationToken)
        ).FirstOrDefault();

      request.Order.CustomerId = customer.Id;
      await _mediator.Publish(new OrderCreatedEvent {Entity = request.Order}, cancellationToken);
      
      return Unit.Value;
    }
  }
}
