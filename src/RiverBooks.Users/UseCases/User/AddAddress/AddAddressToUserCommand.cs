using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.User.AddAddress;

public record AddAddressToUserCommand(string EmailAddress,
                      string Street1,
                      string Street2,
                      string City,
                      string State,
                      string PostalCode,
                      string Country) : IRequest<Result>;
