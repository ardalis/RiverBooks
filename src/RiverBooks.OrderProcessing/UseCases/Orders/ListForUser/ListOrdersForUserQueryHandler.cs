using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.Users.UseCases;

namespace RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;

internal class ListOrdersForUserQueryHandler : IRequestHandler<ListOrdersForUserQuery, Result<List<OrderSummary>>>
{
  private readonly IOrderRepository _orderRepository;

  public ListOrdersForUserQueryHandler(IOrderRepository orderRepository)
  {
    _orderRepository = orderRepository;
  }

  public async Task<Result<List<OrderSummary>>> Handle(ListOrdersForUserQuery request, CancellationToken cancellationToken)
  {
    // look up UserId for EmailAddress

    // add specification to get orders by userid
    //object spec = null!;
    var orders = await _orderRepository.ListAsync();

    var summaries = orders.Select(o => new OrderSummary
    {
      DateCreated = o.DateCreated,
      OrderId = o.Id,
      UserId = o.UserId,
      Total = o.OrderItems.Sum(oi => oi.UnitPrice) // need to .Include OrderItems
    })
      .ToList();

    return summaries;
  }
}
