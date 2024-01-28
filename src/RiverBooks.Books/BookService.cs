namespace RiverBooks.Books;

internal class BookService : IBookService
{
  private readonly IBookRepository _bookRepository;

  public BookService(IBookRepository bookRepository)
  {
    _bookRepository = bookRepository;
  }

  public async Task CreateBookAsync(BookDto newBook)
  {
    var book = new Book(newBook.Id, newBook.Title, newBook.Author, newBook.Price);

    await _bookRepository.AddAsync(book);
    await _bookRepository.SaveChangesAsync();
  }

  public async Task DeleteBookAsync(Guid id)
  {
    var bookToDelete = await _bookRepository.GetByIdAsync(id);

    if (bookToDelete is not null)
    {
      await _bookRepository.DeleteAsync(bookToDelete);
      await _bookRepository.SaveChangesAsync();
    }
  }

  public async Task UpdateBookPriceAsync(Guid bookId, decimal newPrice)
  {
    // validate the newPrice

    var book = await _bookRepository.GetByIdAsync(bookId);

    // handle not found case

    book!.UpdatePrice(newPrice);
    await _bookRepository.SaveChangesAsync();
  }

  public async Task<BookDto> GetBookByIdAsync(Guid id)
  {
    var book = await _bookRepository.GetByIdAsync(id);

    // handle not found case

    return new BookDto(book!.Id, book.Title, book.Author, book.Price);
  }

  public async Task<List<BookDto>> ListBooksAsync()
  {
    List<BookDto> books = (await _bookRepository.ListAsync())
      .Select(book => new BookDto(book.Id, book.Title, book.Author, book.Price))
      .ToList();

    return books;
  }
}
