using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RiverBooks.Users.Data;

namespace RiverBooks.Users.CartEndpoints;

internal class ListCartItems : 
  EndpointWithoutRequest<CartResponse>
{
  private readonly IApplicationUserRepository _userRepository;

  public ListCartItems(IApplicationUserRepository userRepository)
  {
    _userRepository = userRepository;
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
    var user = await _userRepository.GetUserWithCartByEmailAsync(emailAddress ?? "");

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
