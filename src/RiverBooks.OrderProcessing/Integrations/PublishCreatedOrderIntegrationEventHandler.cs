using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Integrations;

internal class PublishCreatedOrderIntegrationEventHandler : INotificationHandler<OrderCreatedEvent>
{
  private readonly IMediator _mediator;

  public PublishCreatedOrderIntegrationEventHandler(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
  {
    var dto = new OrderDetailsDto()
    {
      DateCreated = notification.Order.DateCreated,
      OrderId = notification.Order.Id,
      UserId = notification.Order.UserId,
      OrderItems = notification.Order.OrderItems
      .Select(oi => new OrderItemDetails(oi.BookId,
                                         oi.Quantity,
                                         oi.UnitPrice,
                                         oi.Description))
      .ToList()
    };
    var integrationEvent = new OrderCreatedIntegrationEvent(dto);

    await _mediator.Publish(integrationEvent);

  }
}
