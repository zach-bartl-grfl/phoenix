using System;

namespace phoenix.core.Exceptions
{
  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// but any error messages should be delivered to the user.
  /// </summary>
  public class BadThirdPartyRequestException : Exception
  {
    public string ResponseBody { get; }
    public BadThirdPartyRequestException(string message, string responseBody) : base(message)
    {
      ResponseBody = responseBody;
    }
  }

  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// and any error messages should NOT be delivered to the user, but rather logged.
  /// </summary>
  public class InternalThirdPartyRequestException : Exception
  {
    public string ResponseBody { get; }
    public InternalThirdPartyRequestException(string message, string responseBody = "") : base(message)
    {
      ResponseBody = responseBody;
    }
  }
}
