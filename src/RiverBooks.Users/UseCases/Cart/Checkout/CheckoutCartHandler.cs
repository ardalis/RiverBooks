using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.Users.Interfaces;
using RiverBooks.Users.UseCases.Cart.AddItem;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

internal class CheckoutCartHandler : IRequestHandler<CheckoutCartCommand, Result<Guid>>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly IMediator _mediator;

  public CheckoutCartHandler(IApplicationUserRepository userRepository,
    IMediator mediator)
  {
    _userRepository = userRepository;
    _mediator = mediator;
  }

  public async Task<Result<Guid>> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }

    var items = user.CartItems.Select(item =>
      new OrderItemDetails(item.BookId,
                           item.Quantity,
                           item.UnitPrice,
                           item.Description));

    var createOrderCommand = new CreateOrderCommand(Guid.Parse(user.Id),
      request.shippingAddressId,
      request.billingAddressId,
      items);

    // TODO: Consider replacing with a message-based approach for perf reasons
    var result = await _mediator.Send(createOrderCommand); // synchronous

    // TODO: Clear out the user's cart

    return Result.Success(result.Value.OrderId);
  }
}
