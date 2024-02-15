namespace RiverBooks.SharedKernel;

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents);
}
