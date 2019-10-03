namespace phoenix.core.Domain
{
  public class Customer : LeviathanEntity, IEntity
  {
    /// <inheritdoc />
    public string Id { get; set; }
    
    /// <inheritdoc />
    public string Name { get; set; }
    
    /// <summary>
    /// The customer's home address
    /// </summary>
    public string Address { get; set; }
  }
}
