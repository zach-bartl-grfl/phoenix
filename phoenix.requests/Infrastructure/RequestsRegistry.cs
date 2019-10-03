using MediatR;
using StructureMap;

namespace phoenix.requests.Infrastructure
{
  public class RequestsRegistry : Registry
  {
    public RequestsRegistry()
    {
      Scan(scan =>
      {
        scan.AssemblyContainingType<RequestsRegistry>();
        scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>));
        scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
        scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
        scan.WithDefaultConventions();
      });
      For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
      For<IMediator>().Use<Mediator>();
    }
  }
}
