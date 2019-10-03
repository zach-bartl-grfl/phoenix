using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using phoenix.core.Infrastructure;
using StructureMap;

namespace phoenix.sync
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var host = new HostBuilder()
        .UseServiceProviderFactory(new StructureMapContainerFactory())
        .ConfigureServices(services =>
        {
          var container = ConfigureContainer();
          
          services.AddLogging();
          services.AddHostedService<LeviathanRetry>();
          services.AddHostedService<LeviathanSync>();

          container.Populate(services);
        })
        .ConfigureLogging((hostContext, configLogging) =>
        {
          configLogging.AddConsole();
          configLogging.AddDebug();
        })
        .UseConsoleLifetime()
        .Build();

      await host.RunAsync();
    }

    private static IContainer ConfigureContainer()
    {
      var registry = new Registry();
      registry.IncludeRegistry<CoreRegistry>();
      return new Container(registry);
    }
  }

  public class StructureMapContainerFactory : IServiceProviderFactory<IContainer>
  {
    public IContainer CreateBuilder(IServiceCollection services)
    {
      var registry = new Registry();
      registry.IncludeRegistry<CoreRegistry>();
      var container = new Container(registry);
      container.Populate(services);
      return container;
    }

    public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
    {
      return containerBuilder.GetInstance<IServiceProvider>()
    }
  }
}
