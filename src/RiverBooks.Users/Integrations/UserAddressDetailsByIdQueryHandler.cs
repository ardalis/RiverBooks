using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Integrations;

internal class UserAddressDetailsByIdQueryHandler : IRequestHandler<UserAddressDetailsByIdQuery, Result<UserAddressDetails>>
{
  private readonly IReadOnlyUserStreetAddressRepository _addressRepo;

  public UserAddressDetailsByIdQueryHandler(IReadOnlyUserStreetAddressRepository addressRepo)
  {
    _addressRepo = addressRepo;
  }
  public async Task<Result<UserAddressDetails>> Handle(UserAddressDetailsByIdQuery request,
    CancellationToken ct)
  {
    var address = await _addressRepo.GetById(request.AddressId);

    if (address is null) { return Result.NotFound(); }

    Guid userId = Guid.Parse(address.UserId);

    var details = new UserAddressDetails(userId,
      address.Id,
      address.StreetAddress.Street1,
      address.StreetAddress.Street2,
      address.StreetAddress.City,
      address.StreetAddress.State,
      address.StreetAddress.PostalCode,
      address.StreetAddress.Country);

    return details;
  }

}
