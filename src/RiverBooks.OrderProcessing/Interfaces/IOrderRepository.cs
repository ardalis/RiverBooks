using Ardalis.Specification;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Interfaces;
public interface IOrderRepository
{
  Task<List<Order>> ListAsync();
  Task<List<Order>> ListAsync(ISpecification<Order> specification);
  Task AddAsync(Order order);
  Task SaveChangesAsync();
}
