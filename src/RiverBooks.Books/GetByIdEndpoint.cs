using FastEndpoints;

namespace RiverBooks.Books;

internal class GetByIdEndpoint(IBookService bookService) :
    Endpoint<GetByIdRequest, BookDto>
{
  private readonly IBookService _bookService = bookService;

  public override void Configure()
  {
    Get("/books/{Id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetByIdRequest request,
             CancellationToken cancellationToken = default)
  {
    var book = await _bookService.GetBookByIdAsync(request.Id);

    if(book is null)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    await SendAsync(book);
  }
}
