namespace RiverBooks.Users.Interfaces;

public interface IApplicationUserRepository
{
  Task<ApplicationUser> GetUserByEmailAsync(string email);
  Task<ApplicationUser> GetUserWithCartByEmailAsync(string email);
  Task SaveChangesAsync();
}
