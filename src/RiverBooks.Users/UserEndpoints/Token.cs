using System.Security.Claims;
using FastEndpoints;

namespace RiverBooks.Users.UserEndpoints;

/// <summary>
/// An example non-anonymous page
/// </summary>
sealed class Protected : EndpointWithoutRequest
{
  public override void Configure()
  {
    Get("protected");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(CancellationToken c)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");
    await SendAsync($"You are [{emailAddress}] and you are authorized!");
  }
}
