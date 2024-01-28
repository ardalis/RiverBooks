using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace RiverBooks.Books.BookEndpoints;

internal class ListBooksEndpoint(IBookService bookService) :
    EndpointWithoutRequest<ListBooksResponse>
{
  private readonly IBookService _bookService = bookService;

  public override void Configure()
  {
    Get("/books");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
             CancellationToken cancellationToken = default)
  {
    var books = await _bookService.ListBooksAsync();

    await SendAsync(new ListBooksResponse()
    {
      Books = books
    });
  }
}
