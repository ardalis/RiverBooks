using Ardalis.Result;
using MediatR;
using RiverBooks.Books.Contracts;

namespace RiverBooks.Books.Integrations;

internal class BookDetailsQueryHandler : IRequestHandler<BookDetailsQuery, Result<BookDetailsResponse>>
{
  private readonly IBookService _bookService;

  public BookDetailsQueryHandler(IBookService bookService)
  {
    _bookService = bookService;
  }
  public async Task<Result<BookDetailsResponse>> Handle(BookDetailsQuery request, CancellationToken cancellationToken)
  {
    var book = await _bookService.GetBookByIdAsync(request.BookId);

    if (book is null) { return Result.NotFound(); }

    var response = new BookDetailsResponse(book.Id, book.Title, book.Author, book.Price);

    return response;
  }
}
