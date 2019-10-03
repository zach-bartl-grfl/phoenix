using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using phoenix.core.Domain;
using phoenix.requests.Customers;

namespace phoenix.Controllers
{
  [Route("api/customer")]
  [ApiController]
  public class CustomerController : ControllerBase
  {
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet]
    public ActionResult GetAll()
    {
      return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Customer customer)
    {
      await _mediator.Send(new CreateCustomerCommand {Customer = customer});
      return NoContent();
    }
  }
}
