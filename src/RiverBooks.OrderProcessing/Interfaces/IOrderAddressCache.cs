using Ardalis.Result;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Interfaces;

internal interface IOrderAddressCache
{
  Task<Result<OrderAddress>> GetByIdAsync(Guid id);
  Task<Result> StoreAsync(OrderAddress orderAddress);
}
