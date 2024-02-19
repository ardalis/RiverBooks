using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.GetById;

internal class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDTO>>
{
  private readonly IApplicationUserRepository _userRepository;

  public GetUserByIdHandler(IApplicationUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result<UserDTO>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserByIdAsync(request.UserId);

    if (user is null)
    {
      return Result.NotFound();
    }

    return new UserDTO(Guid.Parse(user!.Id), user.Email!);
  }
}
