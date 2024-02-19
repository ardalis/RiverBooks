using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Interfaces;

public interface IApplicationUserRepository
{
  Task<ApplicationUser> GetUserByEmailAsync(string email);
  Task<ApplicationUser> GetUserByIdAsync(Guid userId);
  Task<ApplicationUser> GetUserWithCartByEmailAsync(string email);
  Task<ApplicationUser> GetUserWithAddressesByEmailAsync(string email);
  Task SaveChangesAsync();
}
