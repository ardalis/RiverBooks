namespace RiverBooks.Users.Contracts;

public record UserAddressDetails(Guid UserId,
                                 Guid AddressId,
                                 string Street1,
                                 string Street2,
                                 string City,
                                 string State,
                                 string PostalCode,
                                 string Country);
