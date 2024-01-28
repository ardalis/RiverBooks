using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Books.Data;

internal class EfBookRepository : IBookRepository
{
  private readonly BookDbContext _dbContext;

  public EfBookRepository(BookDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task AddAsync(Book book)
  {
    _dbContext.Books.Add(book);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Book book)
  {
    _dbContext.Books.Remove(book);
    return Task.CompletedTask;
  }

  public async Task<Book?> GetByIdAsync(Guid id)
  {
    return await _dbContext!.Books.FindAsync(id);
  }

  public async Task<List<Book>> ListAsync()
  {
    return await _dbContext.Books.ToListAsync();
  }

  public async Task SaveChangesAsync()
  {
    await _dbContext.SaveChangesAsync();
  }
}
