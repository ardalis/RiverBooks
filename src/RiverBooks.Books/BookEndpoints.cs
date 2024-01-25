using FastEndpoints;
using Microsoft.AspNetCore.Builder;

namespace RiverBooks.Books;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapGet("/books", (IBookService bookService) =>
        {
            return bookService.GetBooks();
        });
    }
}

public class MyBookResponse
{
    public IEnumerable<BookDto> Books { get; set; } = [];
}
public class MyBookEndpoint(IBookService bookService) : EndpointWithoutRequest<MyBookResponse>
{
    private readonly IBookService _bookService = bookService;

    public override void Configure()
    {
        Get("/api/books");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var books = _bookService.GetBooks();
        await SendAsync(new MyBookResponse { Books = books});
    }
}