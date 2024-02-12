using MediatR;

namespace RiverBooks.Users.Domain;

public abstract class DomainEventBase : INotification
{
  public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
