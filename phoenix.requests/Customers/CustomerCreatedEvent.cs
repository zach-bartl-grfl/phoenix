using phoenix.core.Domain;
using phoenix.requests.Leviathan;
using phoenix.requests.Orders;

namespace phoenix.requests.Customers
{
  /// <summary>
  /// Notification of a new Phoenix Customer
  /// </summary>
  public class CustomerCreatedEvent : ILeviathanEntityNotification<Customer>
  {
  }
}
