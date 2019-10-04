using MediatR;
using phoenix.core.Domain;

namespace phoenix.requests.Leviathan
{
  public class ILeviathanEntityNotification<T> : INotification 
    where T : ILeviathanEntity
  {
    public T Entity { get; set; }
  }
}