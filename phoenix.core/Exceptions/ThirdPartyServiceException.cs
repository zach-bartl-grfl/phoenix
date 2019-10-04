using System;

namespace phoenix.core.Exceptions
{
  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// </summary>
  public class ThirdPartyServiceException : Exception
  {
    public string ServiceMoniker { get; }
    public string ResponseBody { get; }
    public ThirdPartyServiceException(string message, string serviceMoniker, string responseBody) : base(message)
    {
      ServiceMoniker = serviceMoniker;
      ResponseBody = responseBody;
    } 
  }
}