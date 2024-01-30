using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users.CartEndpoints;
internal class AddItem : Endpoint<AddCartItemRequest>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public AddItem(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public override void Configure()
  {
    Post("/cart");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(AddCartItemRequest request,
             CancellationToken cancellationToken = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");
    var user = await _userManager.FindByEmailAsync(emailAddress!);

    // TODO: Where do we get price from?
    var newCartItem = new CartItem(request.BookId, request.Quantity, 1.00m);

    user!.AddItemToCart(newCartItem);

    await _userManager.UpdateAsync(user);

    await SendOkAsync();
  }
}
