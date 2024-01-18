namespace RiverBooks.Books;

public class BookService : IBookService
{
    public IEnumerable<BookDto> GetBooks()
    {
        return [
            new BookDto { Id = Guid.NewGuid(), Title = "The Fellowship of the Ring", Author = "J.R.R. Tolkien" },
            new BookDto { Id = Guid.NewGuid(), Title = "The Two Towers", Author = "J.R.R. Tolkien" },
            new BookDto { Id = Guid.NewGuid(), Title = "The Return of the King", Author = "J.R.R. Tolkien" }];
    }
}

