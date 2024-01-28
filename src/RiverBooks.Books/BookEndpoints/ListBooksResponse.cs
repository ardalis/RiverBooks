namespace RiverBooks.Books.BookEndpoints;

public class ListBooksResponse
{
  public List<BookDto> Books { get; set; } = new();
}
