using Ardalis.Specification;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrdersByUserIdSpec : Specification<Order>
{
  public OrdersByUserIdSpec(Guid userId)
  {
    Query
      .Where(order => order.UserId == userId)
      .Include(order => order.OrderItems);
  }
}
