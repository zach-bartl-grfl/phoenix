namespace phoenix.core.Domain
{
  public class Product : ILeviathanEntity
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string LeviathanId { get; set; }
    public double Price { get; set; }
  }
}