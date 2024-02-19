using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.UseCases.User.Create;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task<Result> Handle(CreateUserCommand command, 
    CancellationToken ct)
  {
    var newUser = new ApplicationUser { Email = command.Email, UserName = command.Email };

    var result = await _userManager.CreateAsync(newUser, command.Password);
    
    // _userManager.GenerateEmailConfirmationTokenAsync(newUser);

    if (result.Succeeded) { return Result.Success(); }

    return Result.Error(result.Errors.Select(e => e.Description).ToArray());
  }
}
