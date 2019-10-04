using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using phoenix.core.Exceptions;

namespace phoenix.Filters
{
  public class InternalThirdPartyServiceExceptionFilter : IExceptionFilter
  {
    public void OnException(ExceptionContext context)
    {
      if (!(context.Exception is InternalThirdPartyServiceException internalException)) return;

      context.ExceptionHandled = true;
      context.Result = new ContentResult
      {
        StatusCode = (int) HttpStatusCode.BadRequest,
        Content = $"Request failed for {internalException.ServiceMoniker} with errors:. " +
                  $"{internalException.Message}"
      };
    }
  }
}
