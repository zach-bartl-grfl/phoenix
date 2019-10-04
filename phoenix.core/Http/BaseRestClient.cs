using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using phoenix.core.Domain;
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
      Dictionary<string, string> parameters = null,
      CancellationToken cancellationToken = default
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
    private const string API_USER_KEY = "ApiUser";
    private const string API_KEY_KEY = "ApiKey";
    
    private readonly ILogger<BaseRestClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _defaultRetryPolicy;
    private readonly NameValueCollection _defaultQuery;

    public BaseRestClient(ILogger<BaseRestClient> logger, ThirdPartyHttpConfig thirdPartyHttpConfig)
    {
      _logger = logger;
      _httpClient = new HttpClient
      {
        BaseAddress = new Uri(thirdPartyHttpConfig.BaseUrl),
        Timeout = TimeSpan.FromSeconds(thirdPartyHttpConfig.QueryTimeout),
      };
      _defaultQuery = HttpUtility.ParseQueryString(string.Empty);
      _defaultQuery[API_USER_KEY] = thirdPartyHttpConfig.ApiUser;
      _defaultQuery[API_KEY_KEY] = thirdPartyHttpConfig.ApiKey;

      //_httpClient.DefaultRequestHeaders.Add(API_USER_KEY, thirdPartyHttpConfig.ApiUser);
      //_httpClient.DefaultRequestHeaders.Add(API_KEY_KEY, thirdPartyHttpConfig.ApiKey);

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
      CancellationToken cancellationToken = default
    ) where T : class
    {
      if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

      var builder = new UriBuilder(_httpClient.BaseAddress)
      {
        Path = url,
        Query = _defaultQuery.ToString()
      };
      

      _logger.LogError($"url: {builder}");
      var response = await _defaultRetryPolicy.ExecuteAsync(async () =>
        await _httpClient.GetAsync(builder.Uri, cancellationToken));

      return await VerifyResponse<T>(url, HttpMethod.Get, response, cancellationToken);
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

      return await VerifyResponse<T>(url, HttpMethod.Post, response, cancellationToken);
    }

    private async Task<T> VerifyResponse<T>(string url,
      HttpMethod method,
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      if (response == null || response.StatusCode == HttpStatusCode.InternalServerError)
      {
        var failureMessage = $"Request {method} {url} could not be completed " +
                             $"(Response: {response?.StatusCode.ToString() ?? "N/A"}).";
        _logger.LogError(failureMessage);
        throw new CleanThirdPartyRequestException(
          failureMessage,
          await response?.Content.ReadAsStringAsync());
      }

      if (response.StatusCode == HttpStatusCode.BadRequest)
      {
        var failureMessage = $"Request {method} {url} could not be completed " +
                             $"(Response: {response.StatusCode.ToString() ?? "N/A"}).";
        _logger.LogError(failureMessage);
        throw new InternalThirdPartyRequestException(
          failureMessage,
          await response.Content.ReadAsStringAsync());
      }

      if (!response.IsSuccessStatusCode)
      {
        var failureMessage = $"Request {method} {url} could not be completed " +
                             $"(Response: {response.StatusCode.ToString() ?? "N/A"}).";
        _logger.LogError($"params: {response.RequestMessage.Headers.GetValues(API_KEY_KEY).FirstOrDefault()}");
        _logger.LogError($"params: {response.RequestMessage.Headers.GetValues(API_USER_KEY).FirstOrDefault()}");
        _logger.LogError(failureMessage);
        throw new InternalThirdPartyRequestException(failureMessage);
      }
      
      return await response.Content.ReadAsAsync<T>(cancellationToken);
    }
  }
}
