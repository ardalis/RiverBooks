using Ardalis.Result;
using Mediator;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.UseCases.Orders.GetDetails;

internal class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, Result<OrderDetails>>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IMediator _mediator;

  public GetOrderDetailsQueryHandler(IOrderRepository orderRepository,
    IMediator mediator)
  {
    _orderRepository = orderRepository;
    _mediator = mediator;
  }

  public async ValueTask<Result<OrderDetails>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
  {
    var userDetailsQuery = new UserDetailsByEmailQuery(request.EmailAddress);
    var userResult = await _mediator.Send(userDetailsQuery, cancellationToken);

    if (userResult.Status != ResultStatus.Ok)
    {
      return Result.Unauthorized();
    }

    // Orders belonging to other users are reported as not found so their existence isn't revealed
    var spec = new OrderByIdForUserSpec(request.OrderId, userResult.Value.UserId);
    var order = await _orderRepository.FirstOrDefaultAsync(spec);

    if (order is null)
    {
      return Result.NotFound();
    }

    return new OrderDetails
    {
      OrderId = order.Id,
      UserId = order.UserId,
      ShippingAddress = ToAddressDetails(order.ShippingAddress),
      BillingAddress = ToAddressDetails(order.BillingAddress),
      DateCreated = order.DateCreated,
      Total = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
      Items = order.OrderItems.Select(oi => new OrderItemDetails
      {
        OrderId = order.Id,
        BookId = oi.BookId,
        Quantity = oi.Quantity,
        UnitPrice = oi.UnitPrice,
        Description = oi.Description
      })
        .ToList()
    };
  }

  private static AddressDetails ToAddressDetails(Address address) =>
    new(address.Street1,
        address.Street2,
        address.City,
        address.State,
        address.PostalCode,
        address.Country);
}
