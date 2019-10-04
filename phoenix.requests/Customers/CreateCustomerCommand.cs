using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using phoenix.core.Data;
using phoenix.core.Domain;

namespace phoenix.requests.Customers
{
  /// <summary>
  /// Requests the creation of a new Phoenix Customer
  /// </summary>
  public class CreateCustomerCommand : IRequest
  {
    public Customer Customer { get; set; }
  }
  
  /// <summary>
  /// Persists the local Phoenix Customer and notifies all listeners of a new Customer event.
  /// </summary>
  public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Unit>
  {
    private readonly IMediator _mediator;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;
    private readonly IMongoCollection<Customer> _collection;

    public CreateCustomerCommandHandler(IMongoDatabaseProvider mongoDatabaseProvider,
      IMediator mediator,
      ILogger<CreateCustomerCommandHandler> logger)
    {
      _mediator = mediator;
      _logger = logger;
      _collection = mongoDatabaseProvider.Collection<Customer>();
    }
    
    public async Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
      try
      {
        await _collection
          .InsertOneAsync(request.Customer, cancellationToken: cancellationToken);
      }
      catch (MongoWriteException)
      {
        _logger.LogError($"Customer already exists: {request.Customer.Id} {request.Customer.LeviathanId}");
        return Unit.Value;
      }
      
      _logger.LogError($"Inserted customer: {request.Customer.Id} {request.Customer.LeviathanId}");
      
      await _mediator.Publish(new CustomerCreatedEvent {Entity = request.Customer}, cancellationToken);
      
      return Unit.Value;
    }
  }
}
