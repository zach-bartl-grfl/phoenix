namespace phoenix.core.Domain
{
  public class Customer : ILeviathanEntity
  {
    /// <inheritdoc />
    public string Id { get; set; }
    
    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string LeviathanId { get; set; }

    /// <summary>
    /// The customer's home address
    /// </summary>
    public string Address { get; set; }
  }
}
