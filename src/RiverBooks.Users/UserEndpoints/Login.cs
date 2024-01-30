using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users.UserEndpoints;

public class Login : Endpoint<UserLoginRequest>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public Login(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }
  public override void Configure()
  {
    Post("/users/login");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UserLoginRequest request, CancellationToken ct)
  {
    var user = await _userManager.FindByEmailAsync(request.Email!);
    if (user is null)
    {
      await SendUnauthorizedAsync();
      return;
    }
    var loginSuccessful = await _userManager.CheckPasswordAsync(user, request.Password);

    if (!loginSuccessful)
    {
      await SendUnauthorizedAsync();
      return;
    }

    var jwtSecret = Config["Auth:JwtSecret"]!;
    var token = JWTBearer.CreateToken(jwtSecret, p => p["EmailAddress"] = user.Email!);
    await SendAsync(token);
  }
}
