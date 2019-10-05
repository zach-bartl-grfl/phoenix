using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using phoenix.core.Domain;
using phoenix.requests.Orders;

namespace phoenix.Controllers
{
  [Route("api/orders")]
  [ApiController]
  public class OrderController : ControllerBase
  {
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
      return new ObjectResult(await _mediator.Send(new OrdersQuery()));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Order order)
    {
      await _mediator.Send(new CreateOrderCommand {Order = order});
      return NoContent();
    }
  }
}
