namespace phoenix.core.Domain
{
  public class Product : LeviathanEntity
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
  }
}