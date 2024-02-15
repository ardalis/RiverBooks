using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.Contracts;

public record UserAddressDetailsByIdQuery(Guid AddressId) : IRequest<Result<UserAddressDetails>>;
