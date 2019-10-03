using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using phoenix.core.Domain;

namespace phoenix.core.Data
{
  /// <summary>
  /// Provides access to mongo collections by accessing the database defined via configuration.
  /// </summary>
  public interface IMongoDatabaseProvider
  {
    IMongoCollection<T> Collection<T>();
  }

  /// <inheritdoc />
  public class MongoDatabaseProvider : IMongoDatabaseProvider
  {
    public IMongoDatabase Phoenix { get; }
    public string DatabaseName { get; }
    
    public MongoDatabaseProvider(IOptionsMonitor<DatabaseConfig> databaseConfig)
    {
      var connectionString = databaseConfig.CurrentValue.ConnectionString;
      var mongoUrl = new MongoUrl(connectionString);
      DatabaseName = mongoUrl.DatabaseName;
      var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
      clientSettings.ConnectTimeout = TimeSpan.FromSeconds(30);
      var client = new MongoClient(clientSettings);
      
      Phoenix = client.GetDatabase(DatabaseName);
    }
    public IMongoCollection<T> Collection<T>()
    
    {
      return Phoenix.GetCollection<T>($"{nameof(T)}s");
    }
  }
}
