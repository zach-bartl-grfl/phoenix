using StructureMap;

namespace phoenix.core.Infrastructure
{
  public class CoreRegistry : Registry
  {
    public CoreRegistry()
    {
      Scan(scan =>
      {
        scan.AssemblyContainingType<CoreRegistry>();
        scan.WithDefaultConventions();
      });
    }
  }
}
