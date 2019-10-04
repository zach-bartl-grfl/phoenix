using phoenix.core.Data;
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
      
      ForSingletonOf<IMongoDatabaseProvider>().ClearAll().Use<MongoDatabaseProvider>();
      For(typeof(IDeadLetterQueueBroker<>)).Use(typeof(DeadLetterQueueBroker<>));
    }
  }
}
