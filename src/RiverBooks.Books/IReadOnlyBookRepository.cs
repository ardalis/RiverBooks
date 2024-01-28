
namespace RiverBooks.Books;

internal interface IReadOnlyBookRepository
{
  Task<Book> GetById(Guid id);
  Task<List<Book>> List();
}
