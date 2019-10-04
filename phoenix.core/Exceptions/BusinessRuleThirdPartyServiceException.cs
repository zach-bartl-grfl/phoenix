namespace phoenix.core.Exceptions
{
  /// <summary>
  /// Denotes that the HTTP request made to an external service was not successful
  /// but any error messages are sound and should be delivered to the user.
  /// </summary>
  public class BusinessRuleThirdPartyServiceException : ThirdPartyServiceException
  {
    public BusinessRuleThirdPartyServiceException(string message, string serviceMoniker, string responseBody) 
      : base(message, serviceMoniker, responseBody)
    {
    }
  }
}