using MediatR;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Data;

internal class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
  private readonly IMediator _mediator;

  public MediatRDomainEventDispatcher(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
  {
    foreach (var entity in entitiesWithEvents)
    {
      var events = entity.DomainEvents.ToArray();
      entity.ClearDomainEvents();
      foreach (var domainEvent in events)
      {
        await _mediator.Publish(domainEvent).ConfigureAwait(false);
      }
    }
  }
}
