using System.Collections.Generic;
using System.Linq;

namespace phoenix.core.Domain
{
  public class Order : ILeviathanEntity
  {
    /// <inheritdoc />
    public string Id { get; set; }
    
    /// <inheritdoc />
    public string Name { get; set; }
    
    /// <inheritdoc />
    public string LeviathanId { get; set; }
    
    /// <summary>
    /// The Customer's Id
    /// </summary>
    public string CustomerId { get; set; }
    
    /// <summary>
    /// The Employee's Id
    /// </summary>
    public string EmployeeId { get; set; }
    
    /// <summary>
    /// The Products purchased by the Customer for this Order
    /// </summary>
    public List<string> Products { get; set; }
    
    /// <summary>
    /// The dollar total of the Products
    /// </summary>
    public double CartTotal { get; set; }
  }
}
