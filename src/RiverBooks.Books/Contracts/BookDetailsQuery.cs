using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using MediatR;

namespace RiverBooks.Books.Contracts;
public record BookDetailsQuery(Guid BookId) : IRequest<Result<BookDetailsResponse>>;

public record BookDetailsResponse(Guid BookId, string Title, string Author, decimal Price);

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
