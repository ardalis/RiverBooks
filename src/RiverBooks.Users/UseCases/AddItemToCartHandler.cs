using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Data;

namespace RiverBooks.Users.UseCases;

internal class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result>
{
  private readonly IApplicationUserRepository _userRepository;

  public AddItemToCartHandler(IApplicationUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }


    // TODO: Where do we get price from?

    var newCartItem = new CartItem(request.BookId, request.Quantity, 1.00m);

    user!.AddItemToCart(newCartItem);

    await _userRepository.SaveChangesAsync();

    return Result.Success();

  }

}
