using MediatR;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Users.Domain;

internal class LogNewAddressesHandler : INotificationHandler<AddressAddedEvent>
{
  private readonly ILogger<LogNewAddressesHandler> _logger;

  public LogNewAddressesHandler(ILogger<LogNewAddressesHandler> logger)
  {
    _logger = logger;
  }
  public Task Handle(AddressAddedEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("[DE Handler]New address added to user {user}: {address}",
      notification.NewAddress.UserId,
      notification.NewAddress.StreetAddress);

    return Task.CompletedTask;
  }
}
