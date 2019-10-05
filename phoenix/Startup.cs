using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using phoenix.core.Domain;
using phoenix.core.Infrastructure;
using phoenix.Filters;
using phoenix.Infrastructure;
using phoenix.requests.Infrastructure;
using StructureMap;

namespace phoenix
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      var container = ConfigureContainer();

      var configBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddEnvironmentVariables();
      var config = configBuilder.Build();
      
      services.Configure<DatabaseConfig>(config.GetSection("DatabaseConfig"));
      services.Configure<LeviathanConfig>(config.GetSection("LeviathanConfig"));
      
      services.AddLogging();
      services
        .AddMvc( options =>
        {
          options.Filters.Add(new BusinessRuleThirdPartyServiceExceptionFilter());
          options.Filters.Add(new InternalThirdPartyServiceExceptionFilter());
        })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      container.Populate(services);
      return container.GetInstance<IServiceProvider>();
    }

    public static IContainer ConfigureContainer()
    {
      var registry = new Registry();
      registry.IncludeRegistry<ApiRegistry>();
      registry.IncludeRegistry<CoreRegistry>();
      registry.IncludeRegistry<RequestsRegistry>();
      return new Container(registry);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
