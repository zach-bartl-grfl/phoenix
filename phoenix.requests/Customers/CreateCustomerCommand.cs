using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
    private readonly IMongoDatabaseProvider _mongoDatabaseProvider;
    private readonly IMediator _mediator;

    public CreateCustomerCommandHandler(IMongoDatabaseProvider mongoDatabaseProvider, IMediator mediator)
    {
      _mongoDatabaseProvider = mongoDatabaseProvider;
      _mediator = mediator;
    }
    
    public async Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
      await _mongoDatabaseProvider.Collection<Customer>()
        .InsertOneAsync(request.Customer, cancellationToken: cancellationToken);
      _mediator.Publish(new CustomerCreatedEvent {Customer = request.Customer});
      return Unit.Value;
    }
  }

}
