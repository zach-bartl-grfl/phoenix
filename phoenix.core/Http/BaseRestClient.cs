using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using phoenix.core.Exceptions;
using Polly;
using Polly.Retry;

namespace phoenix.core.Http
{
  public interface IRequestClient
  {
    /// <summary>
    /// Executes an async GET request against the specified endpoint
    /// </summary>
    /// <param name="url">The endpoint</param>
    /// <param name="parameters">The query parameters</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>The entity</returns>
    Task<T> GetAsync<T>(
      string url,
      Dictionary<string, string> parameters,
      CancellationToken cancellationToken
    ) where T : class;

    /// <summary>
    /// Executes an async POST request against the specified endpoint
    /// </summary>
    /// <param name="url">The endpoint</param>
    /// <param name="body">The request body</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>The modified entity</returns>
    Task<T> PostAsync<T>(
      string url,
      T body,
      CancellationToken cancellationToken
    ) where T : class;
  }

  public class BaseRestClient : IRequestClient
  {
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _defaultRetryPolicy;

    public BaseRestClient(ILogger logger, Func<string> getUrl, Func<TimeSpan> getTimeout)
    {
      _logger = logger;
      _httpClient = new HttpClient
      {
        BaseAddress = new Uri(getUrl()),
        Timeout = getTimeout()
      };

      _defaultRetryPolicy = Policy
        .Handle<TaskCanceledException>()
        .OrResult<HttpResponseMessage>(msg => !msg.IsSuccessStatusCode)
        .WaitAndRetryAsync(new[]
        {
          TimeSpan.FromSeconds(1),
          TimeSpan.FromSeconds(2),
          TimeSpan.FromSeconds(3)
        });
    }

    /// <inheritdoc />
    public async Task<T> GetAsync<T>(
      string url,
      Dictionary<string, string> parameters,
      CancellationToken cancellationToken
    ) where T : class
    {
      if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

      var response = await _defaultRetryPolicy.ExecuteAsync(async () =>
        await _httpClient.GetAsync(url, cancellationToken));

      return await VerifyResponse<T>(url, response, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T> PostAsync<T>(
      string url,
      T body,
      CancellationToken cancellationToken
    ) where T : class
    {
      if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

      var response = await _defaultRetryPolicy.ExecuteAsync(async () =>
        await _httpClient.PostAsJsonAsync(url, body, cancellationToken));

      return await VerifyResponse<T>(url, response, cancellationToken);
    }

    private async Task<T> VerifyResponse<T>(string url, HttpResponseMessage response, CancellationToken cancellationToken)
    {
      if (response == null || response.StatusCode == HttpStatusCode.InternalServerError)
      {
        var failureMessage = $"Request POST {url} could not be completed " +
                             $"(Response: {response?.StatusCode.ToString() ?? "N/A"}.";
        _logger.LogError(failureMessage);
        throw new BadThirdPartyRequestException(
          failureMessage,
          await response?.Content.ReadAsStringAsync());
      }

      if (response.StatusCode == HttpStatusCode.BadRequest)
      {
        var failureMessage = $"Request POST {url} could not be completed " +
                             $"(Response: {response.StatusCode.ToString() ?? "N/A"}.";
        _logger.LogError(failureMessage);
        throw new InternalThirdPartyRequestException(
          failureMessage,
          await response.Content.ReadAsStringAsync());
      }

      if (!response.IsSuccessStatusCode)
      {
        var failureMessage = $"Request POST {url} could not be completed " +
                             $"(Response: {response.StatusCode.ToString() ?? "N/A"}.";
        _logger.LogError(failureMessage);
        throw new InternalThirdPartyRequestException(failureMessage);
      }
      
      return await response.Content.ReadAsAsync<T>(cancellationToken);
    }
  }
}
