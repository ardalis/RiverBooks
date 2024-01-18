
namespace RiverBooks.Books
{
    public interface IBookService
    {
        IEnumerable<BookDto> GetBooks();
    }
}