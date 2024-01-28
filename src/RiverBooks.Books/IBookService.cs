namespace RiverBooks.Books;

internal interface IBookService
{
  Task<List<BookDto>> ListBooks();
  Task<BookDto> GetBookById(Guid id);

  Task CreateBook(BookDto newBook);
  Task DeleteBook(Guid id);
  Task UpdateBookPrice(Guid bookId, decimal newPrice);
}
