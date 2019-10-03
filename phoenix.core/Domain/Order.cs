using System.Collections.Generic;
using System.Linq;

namespace phoenix.core.Domain
{
  public class Order : LeviathanEntity
  {
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public string EmployeeId { get; set; }
    public List<Product> Products { get; set; }
    public double CartTotal => Products.Sum(p => p.Price);
  }
}