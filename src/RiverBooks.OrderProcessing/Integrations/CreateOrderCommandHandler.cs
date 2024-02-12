using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Integrations;
internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDetailsResponse>>
{
  private readonly IOrderRepository _orderRepository;

  public CreateOrderCommandHandler(IOrderRepository orderRepository)
  {
    _orderRepository = orderRepository;
  }
  public async Task<Result<OrderDetailsResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
  {
    // need to look up user addresses in materialized view
    // addresses are immutable - if we have the id we know we have the correct address

    // need to look up book details (via mediatR)

    // need to create order - use OrderFactory

    var address = new Address("street1", "street2", "city", "state", "zip", "country");
    var order = Order.Factory.Create(command.UserId, address, address, new List<OrderItem>());

    await _orderRepository.AddAsync(order);
    await _orderRepository.SaveChangesAsync();

    return new OrderDetailsResponse(order.Id);
  }
}
