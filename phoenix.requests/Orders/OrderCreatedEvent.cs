using phoenix.core.Domain;
using phoenix.requests.Leviathan;

namespace phoenix.requests.Orders
{
  /// <summary>
  /// Notification of a new Phoenix Order
  /// </summary>
  public class OrderCreatedEvent : ILeviathanEntityNotification<Order>
  {
  }
}
