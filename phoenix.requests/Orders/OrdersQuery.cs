using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using phoenix.core.Data;
using phoenix.core.Domain;

namespace phoenix.requests.Orders
{
  public class OrdersQuery : IRequest<List<Order>>
  {
  }
  
  public class CustomersQueryHandler : IRequestHandler<OrdersQuery, List<Order>>
  {
    private readonly IMongoCollection<Order> _collection;

    public CustomersQueryHandler(IMongoDatabaseProvider mongoDatabaseProvider)
    {
      _collection = mongoDatabaseProvider.Collection<Order>();
    }
    
    public async Task<List<Order>> Handle(OrdersQuery request, CancellationToken cancellationToken)
    {
      return (await _collection.FindAsync(Builders<Order>.Filter.Empty,
        cancellationToken: cancellationToken)).ToList();
    }
  }
}
