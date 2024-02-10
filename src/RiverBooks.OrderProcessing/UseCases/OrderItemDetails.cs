namespace RiverBooks.Users.UseCases;

internal record OrderItemDetails
{
  public Guid OrderId { get; set; }
  public Guid BookId { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public string Description { get; set; } = string.Empty;
}

