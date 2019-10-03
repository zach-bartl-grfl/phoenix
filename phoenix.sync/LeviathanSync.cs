using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace phoenix.sync
{
  /// <summary>
  /// Pulls and syncs new entities not present in Phoenix from the Leviathan Traceability API
  /// </summary>
  public class LeviathanSync : BackgroundService
  {
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      throw new System.NotImplementedException();
    }
  }
}
