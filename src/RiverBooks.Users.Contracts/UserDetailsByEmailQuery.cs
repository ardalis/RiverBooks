using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.Contracts;

public record UserDetailsByEmailQuery(string EmailAddress) : IRequest<Result<UserDetailsResponse>>;


