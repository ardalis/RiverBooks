using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Integrations;

internal class UserAddressIntegrationEventDispatcherHandler : INotificationHandler<AddressAddedEvent>
{
  private readonly IMediator _mediator;
  private readonly ILogger<UserAddressIntegrationEventDispatcherHandler> _logger;

  public UserAddressIntegrationEventDispatcherHandler(
    IMediator mediator,
    ILogger<UserAddressIntegrationEventDispatcherHandler> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public async Task Handle(AddressAddedEvent notification, CancellationToken cancellationToken)
  {
    Guid userId = Guid.Parse(notification.NewAddress.UserId);
    
    var addressDetails = new UserAddressDetails(userId,
      notification.NewAddress.Id,
      notification.NewAddress.StreetAddress.Street1,
      notification.NewAddress.StreetAddress.Street2,
      notification.NewAddress.StreetAddress.City,
      notification.NewAddress.StreetAddress.State,
      notification.NewAddress.StreetAddress.PostalCode,
      notification.NewAddress.StreetAddress.Country);

    await _mediator!.Publish(new NewUserAddressAddedIntegrationEvent(addressDetails));

    _logger.LogInformation("[DE Handler]New address integration event sent for {user}: {address}",
      notification.NewAddress.UserId,
      notification.NewAddress.StreetAddress);
  }
}
