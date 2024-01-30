using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;

namespace RiverBooks.Users.Endpoints;

public class CreateToken : EndpointWithoutRequest
{
  public override void Configure()
  {
    Get("token");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var jwtSecret = Config["Auth:JwtSecret"]!;
    await SendAsync(JWTBearer.CreateToken(jwtSecret, p => p["UserId"] = "001"));
  }
}

sealed class Protected : EndpointWithoutRequest
{
  public override void Configure()
  {
    Get("protected");
    Claims("UserId");
  }

  public override async Task HandleAsync(CancellationToken c)
  {
    var userId = User.FindFirstValue("UserId");
    await SendAsync($"You are [{userId}] and is authorized!");
  }
}
