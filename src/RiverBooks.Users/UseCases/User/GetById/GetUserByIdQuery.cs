using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.User.GetById;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDTO>>;
