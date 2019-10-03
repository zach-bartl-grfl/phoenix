using MediatR;
using phoenix.core.Domain;

namespace phoenix.requests.Customers
{
  /// <summary>
  /// Notification of a new Phoenix Customer
  /// </summary>
  public class CustomerCreatedEvent : INotification
  {
    public Customer Customer { get; set; }
  }
}
