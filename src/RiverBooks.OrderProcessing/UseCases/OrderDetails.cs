namespace RiverBooks.Users.UseCases;

internal record OrderDetails
{
  public Guid UserId { get; set; }
  public AddressDetails ShippingAddress { get; set; } = default!;
  public AddressDetails BillingAddress { get; set; } = default!;
  public DateTimeOffset DateCreated { get; set; }
  public DateTimeOffset? DateShipped { get; set; }
  public decimal Total { get; set; }
  public List<OrderItemDetails> Items { get; set; } = new();
}
