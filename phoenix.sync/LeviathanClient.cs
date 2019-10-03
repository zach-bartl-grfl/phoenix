using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using phoenix.core.Data;
using phoenix.core.Domain;
using phoenix.core.Http;

namespace phoenix.sync
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
    public LeviathanClient(ILogger logger,
      IOptionsMonitor<LeviathanConfig> leviathanConfig,
      IDeadLetterQueueBroker<Customer> queue) : base(
      logger,
      () => leviathanConfig.CurrentValue.BaseUrl,
      () => TimeSpan.FromSeconds(leviathanConfig.CurrentValue.QueryTimeout)) {}
}
