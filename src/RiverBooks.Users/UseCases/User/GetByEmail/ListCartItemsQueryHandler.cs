using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.GetByEmail;

internal class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, Result<UserDTO>>
{
  private readonly IApplicationUserRepository _userRepository;

  public GetUserByEmailHandler(IApplicationUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result<UserDTO>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.NotFound();
    }

    return new UserDTO(Guid.Parse(user!.Id), user.Email!);
  }
}
