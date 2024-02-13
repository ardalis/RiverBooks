using System.Numerics;
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Integrations;

internal class AddressCacheUpdater : INotificationHandler<NewUserAddressAddedIntegrationEvent>
{
  private readonly IOrderAddressCache _addressCache;
  private readonly ILogger<AddressCacheUpdater> _logger;

  public AddressCacheUpdater(IOrderAddressCache addressCache,
    ILogger<AddressCacheUpdater> logger)
  {
    _addressCache = addressCache;
    _logger = logger;
  }

  public async Task Handle(NewUserAddressAddedIntegrationEvent notification, CancellationToken ct)
  {
    var orderAddress = new OrderAddress(notification.Details.AddressId,
      new Address(notification.Details.Street1,
        notification.Details.Street2,
        notification.Details.City,
        notification.Details.State,
        notification.Details.PostalCode,
        notification.Details.Country));

    await _addressCache.StoreAsync(orderAddress);

    _logger.LogInformation("Cached updated with new address {address}", orderAddress);
  }
}
