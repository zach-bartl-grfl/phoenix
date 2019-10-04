namespace phoenix.core.Domain
{
  /// <summary>
  /// Defines an entity that should be synced with the Leviathan Traceability API
  /// and therefore should (eventually) have a reference to a Leviathan Id
  /// </summary>
  public interface ILeviathanEntity : IEntity
  {
    /// <inheritdoc />
    string Id { get; set; }
    
    /// <inheritdoc />
    string Name { get; set; }
    
    /// <summary>
    /// The LeviathanId for this Entity
    /// </summary>
    string LeviathanId { get; set; }
  }
}
