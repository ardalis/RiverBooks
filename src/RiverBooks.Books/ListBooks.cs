using FastEndpoints;

namespace RiverBooks.Books;

public class MyBookResponse
{
    public IEnumerable<BookDto> Books { get; set; } = [];
}
public class ListBooks(IBookService bookService) : EndpointWithoutRequest<MyBookResponse>
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