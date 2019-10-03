namespace phoenix.core.Domain
{
  public interface IEntity
  {
    /// <summary>
    /// The local database identifier of this entity.
    /// </summary>
    string Id { get; set; }
    
    /// <summary>
    /// The generic name of this entity.
    /// </summary>
    string Name { get; set; }
  }
}
