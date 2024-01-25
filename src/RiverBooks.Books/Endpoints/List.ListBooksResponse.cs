namespace RiverBooks.Books.Endpoints;

public class ListBooksResponse
{
    public IEnumerable<BookDto> Books { get; set; } = [];
}
