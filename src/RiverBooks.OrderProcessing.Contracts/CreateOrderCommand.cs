using Ardalis.Result;
using MediatR;

namespace RiverBooks.OrderProcessing.Contracts;

public class CreateOrderCommand : IRequest<Result<OrderDetailsResponse>>
{
  public CreateOrderCommand(Guid userId,
    Guid shippingAddressId,
    Guid billingAddressId,
    IEnumerable<OrderItemDetails> orderItems)
  {
    UserId = userId;
    ShippingAddressId = shippingAddressId;
    BillingAddressId = billingAddressId;
    OrderItems = new List<OrderItemDetails>(orderItems);
  }

  public Guid UserId { get; set;  }
  public Guid ShippingAddressId { get; set; }
  public Guid BillingAddressId { get; set;  }
  public List<OrderItemDetails> OrderItems { get; }
}
