using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.Contracts;

public record UserDetailsByIdQuery(Guid UserId) : IRequest<Result<UserDetailsResponse>>;


