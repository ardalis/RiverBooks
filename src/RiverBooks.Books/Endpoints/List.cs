using FastEndpoints;

namespace RiverBooks.Books.Endpoints;
public class List(IBookService bookService) : 
    EndpointWithoutRequest<ListBooksResponse>
{
    private readonly IBookService _bookService = bookService;

    public override void Configure()
    {
        Get("/books");
        
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var books = _bookService.GetBooks();
        await SendAsync(new ListBooksResponse { Books = books });
    }
}