using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using phoenix.core.Data;
using phoenix.core.Domain;

namespace phoenix.requests.Customers
{
  public class CustomersQuery : IRequest<List<Customer>>
  {
  }
  
  public class CustomersQueryHandler : IRequestHandler<CustomersQuery, List<Customer>>
  {
    private readonly IMongoCollection<Customer> _collection;

    public CustomersQueryHandler(IMongoDatabaseProvider mongoDatabaseProvider)
    {
      _collection = mongoDatabaseProvider.Collection<Customer>();
    }
    
    public async Task<List<Customer>> Handle(CustomersQuery request, CancellationToken cancellationToken)
    {
      return (await _collection.FindAsync(Builders<Customer>.Filter.Empty,
        cancellationToken: cancellationToken)).ToList();
    }
  }
}
