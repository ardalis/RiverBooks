using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RiverBooks.Users.Data;

namespace RiverBooks.Users.CartEndpoints;
internal class AddItem : Endpoint<AddCartItemRequest>
{
  private readonly IApplicationUserRepository _userRepository;

  public AddItem(IApplicationUserRepository userRepository)
  {
    _userRepository = userRepository;
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
    var user = await _userRepository.GetUserWithCartByEmailAsync(emailAddress!);

    // TODO: Where do we get price from?

    var newCartItem = new CartItem(request.BookId, request.Quantity, 1.00m);

    user!.AddItemToCart(newCartItem);

    await _userRepository.SaveChangesAsync();

    await SendOkAsync();
  }
}
