using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users.CartEndpoints;

internal class ListCartItems : 
  EndpointWithoutRequest<CartResponse>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public ListCartItems(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public override void Configure()
  {
    Get("/cart");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");
    var user = await _userManager.FindByEmailAsync(emailAddress!);

    if (user is null)
    {
      await SendUnauthorizedAsync();
    }
    else
    {
      var cartResponse = new CartResponse()
      {
        CartItems = user!.CartItems!
                          .Select(item => new CartItemDto(item.Id, Guid.Empty, "", item.Quantity, item.UnitPrice))
                          .ToList()
      };

      await SendAsync(cartResponse);
    }
  }
}
