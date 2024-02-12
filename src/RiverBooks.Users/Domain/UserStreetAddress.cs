using Ardalis.GuardClauses;

namespace RiverBooks.Users.Domain;

internal class UserStreetAddress
{
  public UserStreetAddress(string userId, Address streetAddress)
  {
    UserId = Guard.Against.NullOrEmpty(userId);
    StreetAddress = Guard.Against.Null(streetAddress);
  }

  private UserStreetAddress() { } // EF

  public Guid Id { get; private set; } = Guid.NewGuid();
  public string UserId { get; private set; } = string.Empty;
  public Address StreetAddress { get; private set; } = default!;
}
