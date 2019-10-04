using System;

namespace phoenix.core.Exceptions
{
  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// </summary>
  public class ThirdPartyRequestException : Exception
  {
    public string ResponseBody { get; }
    public ThirdPartyRequestException(string message, string responseBody) : base(message)
    {
      ResponseBody = responseBody;
    } 
  }
  
  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// but any error messages are sound and should be delivered to the user.
  /// </summary>
  public class CleanThirdPartyRequestException : ThirdPartyRequestException
  {
    public CleanThirdPartyRequestException(string message, string responseBody) 
      : base(message, responseBody)
    {
    }
  }

  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// and any error messages should NOT be delivered to the user, but rather logged.
  /// </summary>
  public class InternalThirdPartyRequestException : ThirdPartyRequestException
  {
    public InternalThirdPartyRequestException(string message, string responseBody = "") 
      : base(message, responseBody)
    {
    }
  }
}
