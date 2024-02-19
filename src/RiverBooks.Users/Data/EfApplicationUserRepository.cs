using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Data;

internal class EfApplicationUserRepository : IApplicationUserRepository
{
  private readonly UsersDbContext _dbContext;

  public EfApplicationUserRepository(UsersDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<ApplicationUser> GetUserByEmailAsync(string email)
  {
    return _dbContext.ApplicationUsers
      .SingleAsync(u => u.Email == email);
  }

  public Task<ApplicationUser> GetUserByIdAsync(Guid userId)
  {
    return _dbContext.ApplicationUsers
      .SingleAsync(u => u.Id == userId.ToString());
  }

  public Task<ApplicationUser> GetUserWithAddressesByEmailAsync(string email)
  {
    return _dbContext.ApplicationUsers
      .Include(user => user.Addresses)
      .SingleAsync(u => u.Email == email);
  }

  public Task<ApplicationUser> GetUserWithCartByEmailAsync(string email)
  {
    return _dbContext.ApplicationUsers
      .Include(user => user.CartItems)
      .SingleAsync(u => u.Email == email);
  }

  public async Task SaveChangesAsync()
  {
    await _dbContext.SaveChangesAsync();
  }
}
