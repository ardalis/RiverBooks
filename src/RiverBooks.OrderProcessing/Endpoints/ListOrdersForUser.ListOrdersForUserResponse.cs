using RiverBooks.OrderProcessing.Endpoints;

namespace RiverBooks.Users.CartEndpoints;

public class ListOrdersForUserResponse 
{
  public List<OrderSummary> Orders { get; set; } = new();
}

