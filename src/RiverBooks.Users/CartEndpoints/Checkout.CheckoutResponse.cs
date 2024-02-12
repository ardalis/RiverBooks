namespace RiverBooks.Users.CartEndpoints;

public record CheckoutResponse
{
  Guid NewOrderId { get; set; }
}
