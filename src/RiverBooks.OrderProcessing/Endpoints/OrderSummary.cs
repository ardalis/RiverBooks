namespace RiverBooks.OrderProcessing.Endpoints;

public record OrderSummary
{
  public Guid OrderId { get; set; }
  public Guid UserId { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public decimal Total { get; set; }
}
