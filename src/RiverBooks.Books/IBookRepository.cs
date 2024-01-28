namespace RiverBooks.Books;

internal interface IBookRepository : IReadOnlyBookRepository
{
  Task AddAsync(Book book);
  Task DeleteAsync(Book book);
  Task UpdateAsync(Book book);
  Task SaveChangesAsync();
}
