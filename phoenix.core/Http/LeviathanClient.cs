using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using phoenix.core.Domain;

namespace phoenix.core.Http
{
  /// <summary>
  /// Implementation of BaseRestClient for the Leviathan Traceability API
  /// </summary>
  public interface ILeviathanClient : IRequestClient
  {
  }

  /// <inheritdoc />
  public class LeviathanClient : BaseRestClient, ILeviathanClient
  {
    public LeviathanClient(
      ILogger<LeviathanClient> logger,
      IOptionsMonitor<LeviathanConfig> leviathanConfig) : base(
      logger,
      leviathanConfig.CurrentValue)
    {
    }
  }
}
