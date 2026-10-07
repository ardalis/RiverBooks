using RiverBooks.OrderProcessing.Endpoints;

namespace RiverBooks.Users.CartEndpoints;

internal class ListOrdersForUserResponse 
{
  public List<OrderSummary> Orders { get; set; } = new();
}

