namespace RiverBooks.Users.UserEndpoints;

internal class AddressListResponse
{
  public List<UserAddressDto> Addresses { get; set; } = new();
}
