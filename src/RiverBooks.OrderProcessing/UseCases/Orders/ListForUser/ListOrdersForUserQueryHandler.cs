using Ardalis.Result;
using Mediator;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;

public class ListOrdersForUserQueryHandler : IRequestHandler<ListOrdersForUserQuery, Result<List<OrderSummary>>>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IMediator _mediator;

  public ListOrdersForUserQueryHandler(IOrderRepository orderRepository,
    IMediator mediator)
  {
    _orderRepository = orderRepository;
    _mediator = mediator;
  }

  public async ValueTask<Result<List<OrderSummary>>> Handle(ListOrdersForUserQuery request, CancellationToken cancellationToken)
  {
    var userDetailsQuery = new UserDetailsByEmailQuery(request.EmailAddress);
    var userResult = await _mediator.Send(userDetailsQuery, cancellationToken);

    if (userResult.Status != ResultStatus.Ok)
    {
      return Result.Unauthorized();
    }

    var spec = new OrdersByUserIdSpec(userResult.Value.UserId);
    var orders = await _orderRepository.ListAsync(spec);

    var summaries = orders.Select(o => new OrderSummary
    {
      DateCreated = o.DateCreated,
      OrderId = o.Id,
      UserId = o.UserId,
      Total = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
    })
      .ToList();

    return summaries;
  }
}
