using Ardalis.Result;
using Mediator;
using RiverBooks.Users.Interfaces;
using RiverBooks.Users.UserEndpoints;

namespace RiverBooks.Users.UseCases.Addresses.GetById;

public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Result<UserAddressDto>>
{
  private readonly IReadOnlyUserStreetAddressRepository _addressRepository;

  public GetAddressByIdQueryHandler(IReadOnlyUserStreetAddressRepository addressRepository)
  {
    _addressRepository = addressRepository;
  }

  public async ValueTask<Result<UserAddressDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
  {
    var address = await _addressRepository.GetById(request.AddressId);

    if (address is null)
    {
      return Result.NotFound();
    }

    return new UserAddressDto(address.Id,
      address.StreetAddress.Street1,
      address.StreetAddress.Street2,
      address.StreetAddress.City,
      address.StreetAddress.State,
      address.StreetAddress.PostalCode,
      address.StreetAddress.Country);
  }
}
