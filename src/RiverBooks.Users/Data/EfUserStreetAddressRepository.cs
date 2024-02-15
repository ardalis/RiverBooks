using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Data;

internal class EfUserStreetAddressRepository : IReadOnlyUserStreetAddressRepository
{
  private readonly UsersDbContext _dbContext;

  public EfUserStreetAddressRepository(UsersDbContext _dbContext)
  {
    this._dbContext = _dbContext;
  }

  public Task<UserStreetAddress?> GetById(Guid userStreetAddressId)
  {
    return _dbContext.UserStreetAddresses
      .SingleOrDefaultAsync(a => a.Id == userStreetAddressId);
  }
}
