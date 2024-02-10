using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Interfaces;
internal interface IOrderRepository
{
  Task<List<Order>> ListAsync();
}
