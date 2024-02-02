using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.Data;

namespace RiverBooks.Users.UseCases;

internal class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly IMediator _mediator;

  public AddItemToCartHandler(IApplicationUserRepository userRepository,
    IMediator mediator)
  {
    _userRepository = userRepository;
    _mediator = mediator;
  }

  public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }


    // TODO: Where do we get price from?
    var bookDetailsQuery = new BookDetailsQuery(request.BookId);
    var result = await _mediator.Send(bookDetailsQuery);

    var bookDetails = result.Value;

    var newCartItem = new CartItem(request.BookId, request.Quantity, bookDetails.Price, $"{bookDetails.Title} by {bookDetails.Author}");

    user!.AddItemToCart(newCartItem);

    await _userRepository.SaveChangesAsync();

    return Result.Success();

  }

}
