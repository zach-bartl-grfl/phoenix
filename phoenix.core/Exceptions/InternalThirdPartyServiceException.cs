namespace phoenix.core.Exceptions
{
  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// and any error messages should NOT be delivered to the user, but rather logged.
  /// </summary>
  public class InternalThirdPartyServiceException : ThirdPartyServiceException
  {
    public InternalThirdPartyServiceException(string message, string serviceMoniker, string responseBody = "") 
      : base(message, serviceMoniker, responseBody)
    {
    }
  }
}
