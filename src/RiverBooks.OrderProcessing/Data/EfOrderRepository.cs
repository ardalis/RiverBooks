using Microsoft.EntityFrameworkCore;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Data;

internal class EfOrderRepository : IOrderRepository
{
  private readonly OrderProcessingDbContext _dbContext;

  public EfOrderRepository(OrderProcessingDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<List<Order>> ListAsync()
  {
    return await _dbContext.Orders.ToListAsync();
  }
}
