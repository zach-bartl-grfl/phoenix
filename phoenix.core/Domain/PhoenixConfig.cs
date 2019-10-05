namespace phoenix.core.Domain
{
  public class DatabaseConfig
  {
    public string ConnectionString { get; set; }
  }

  public class LeviathanConfig : ThirdPartyHttpConfig
  {
  }
  
  public class ThirdPartyHttpConfig
  {
    public string BaseUrl { get; set; }
    public string ApiUser { get; set; }
    public string ApiKey { get; set; }
    public int QueryTimeout { get; set; }
    public int SyncDelay { get; set; }
    public int RetryDelay { get; set; }
  }
}
