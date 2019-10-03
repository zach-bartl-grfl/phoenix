namespace phoenix.core.Domain
{
  /// <summary>
  /// Defines an entity that should be synced with the Leviathan Traceability API
  /// and therefore should (eventually) have a reference to a Leviathan Id
  /// </summary>
  public abstract class LeviathanEntity
  {
    public string LeviathanId { get; set; }
  }
}
