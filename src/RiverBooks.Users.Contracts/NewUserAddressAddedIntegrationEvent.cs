using RiverBooks.SharedKernel;

namespace RiverBooks.Users.Contracts;

public record NewUserAddressAddedIntegrationEvent(UserAddressDetails Details)
  : IntegrationEventBase;
