using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Interfaces;

internal interface IHaveDomainEvents
{
  IEnumerable<DomainEventBase> DomainEvents { get; }
  void ClearDomainEvents();
}
