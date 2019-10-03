using StructureMap;

namespace phoenix.Infrastructure
{
  public class ApiRegistry : Registry
  {
    public ApiRegistry()
    {
      Scan(scan =>
      {
        scan.AssemblyContainingType<ApiRegistry>();
        scan.WithDefaultConventions();
      });
    }
  }
}
