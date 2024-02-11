using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.User.GetByEmail;

public record GetUserByEmailQuery(string EmailAddress) : IRequest<Result<UserDTO>>;
