using FastEndpoints;

namespace RiverBooks.Books.BookEndpoints;

internal class Create(IBookService bookService) :
    Endpoint<CreateBookRequest, BookDto>
{
  private readonly IBookService _bookService = bookService;

  public override void Configure()
  {
    Post("/books");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateBookRequest request,
             CancellationToken cancellationToken = default)
  {
    var newBookDto = new BookDto(request.Id ?? Guid.NewGuid(),
      request.Title,
      request.Author,
      request.Price);

    await _bookService.CreateBookAsync(newBookDto);

    await SendCreatedAtAsync<GetById>(new { newBookDto.Id }, newBookDto);
  }
}
