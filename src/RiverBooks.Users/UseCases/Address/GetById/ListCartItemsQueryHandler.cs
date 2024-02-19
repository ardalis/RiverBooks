using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Interfaces;
using RiverBooks.Users.UseCases.User;
using RiverBooks.Users.UseCases.User.GetByEmail;

namespace RiverBooks.Users.UseCases.Addresses.GetById;

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
