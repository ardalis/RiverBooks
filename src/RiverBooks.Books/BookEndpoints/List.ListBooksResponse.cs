namespace RiverBooks.Books.BookEndpoints;

internal class ListBooksResponse
{
  public List<BookDto> Books { get; set; } = new();
}
