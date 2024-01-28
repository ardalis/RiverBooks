using System.Linq;

namespace RiverBooks.Books;

internal class BookService : IBookService
{
  private readonly IBookRepository _bookRepository;

  public BookService(IBookRepository bookRepository)
  {
    _bookRepository = bookRepository;
  }

  public async Task CreateBook(BookDto newBook)
  {
    var book = new Book(newBook.Id, newBook.Title, newBook.Author, newBook.Price);

    await _bookRepository.Add(book);
    await _bookRepository.SaveChanges();
  }

  public async Task DeleteBook(Guid id)
  {
    var bookToDelete = await _bookRepository.GetById(id);

    if (bookToDelete is not null)
    {
      await _bookRepository.Delete(bookToDelete);
      await _bookRepository.SaveChanges();
    }
  }

  public async Task<BookDto> GetBookById(Guid id)
  {
    var book = await _bookRepository.GetById(id);

    return new BookDto(book.Id, book.Title, book.Author, book.Price);
  }

  //public List<BookDto> ListBooks()
  //{
  //  return [
  //      new BookDto(Guid.NewGuid(), "The Fellowship of the Ring", "J.R.R. Tolkien"),
  //    new BookDto(Guid.NewGuid(), "The Two Towers", "J.R.R. Tolkien"),
  //    new BookDto(Guid.NewGuid(), "The Return of the King", "J.R.R. Tolkien")
  //  ];
  //}

  public async Task UpdateBookPrice(Guid bookId, decimal newPrice)
  {
    // validate the newPrice

    var book = await _bookRepository.GetById(bookId);

    book.UpdatePrice(newPrice);
  }

  public async Task<List<BookDto>> ListBooks()
  {
    List<BookDto> books = (await _bookRepository.List())
      .Select(book => new BookDto(book.Id, book.Title, book.Author, book.Price))
      .ToList();

    return books;
  }
}
