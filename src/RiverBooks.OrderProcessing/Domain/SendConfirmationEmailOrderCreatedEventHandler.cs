using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

internal class SendConfirmationEmailOrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
  private readonly IMediator _mediator;

  public SendConfirmationEmailOrderCreatedEventHandler(IMediator mediator)
  {
    _mediator = mediator;
  }
  public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
  {
    // get user email from id
    var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

    var result = await _mediator.Send(userByIdQuery);

    if (!result.IsSuccess)
    {
      // TODO: Add logging
      return;
    }

    var userEmail = result.Value.EmailAddress;

    var command = new SendEmailCommand()
    {
      To = userEmail,
      From = "noreply@test.com",
      Subject = "Your RiverBooks Purchase",
      Body = $"You bought {notification.Order.OrderItems.Count} items."
    };
    Guid emailId = await _mediator.Send(command);

    // TODO: store emailId
  }
}
