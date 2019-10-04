namespace phoenix.core.Domain
{
  public class Employee : ILeviathanEntity
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string LeviathanId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public EmployeePosition Position { get; set; }
  }
}
