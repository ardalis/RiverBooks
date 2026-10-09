
namespace RiverBooks.OrderProcessing.Endpoints;

internal class ListOrdersForUserResponse 
{
  public List<OrderSummary> Orders { get; set; } = new();
}

