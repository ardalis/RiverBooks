using Ardalis.Specification;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderByIdForUserSpec : Specification<Order>, ISingleResultSpecification<Order>
{
  public OrderByIdForUserSpec(Guid orderId, Guid userId)
  {
    Query
      .Where(order => order.Id == orderId && order.UserId == userId)
      .Include(order => order.OrderItems);
  }
}
