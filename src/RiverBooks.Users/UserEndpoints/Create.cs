using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.Domain;
using RiverBooks.Users.UseCases.Cart.AddItem;

namespace RiverBooks.Users.UserEndpoints;

internal sealed class Create : Endpoint<CreateUserRequest>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public Create(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public override void Configure()
  {
    Post("/users");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
  {
    var newUser = new ApplicationUser { Email = req.Email, UserName = req.Email };

    var result = await _userManager.CreateAsync(newUser, req.Password);

    if (!result.Succeeded)
    {
      await SendErrorsAsync();
    }
    await SendOkAsync();
  }
}
