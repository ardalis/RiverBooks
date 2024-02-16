namespace RiverBooks.OrderProcessing.Contracts;

/// <summary>
/// Basic details of the order
/// TODO: Include address info for geographic specific reports to use
/// </summary>
public class OrderDetailsDto
{
  public Guid OrderId { get; set; }
  public Guid UserId { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public List<OrderItemDetails> OrderItems { get; set; } = new();
}
