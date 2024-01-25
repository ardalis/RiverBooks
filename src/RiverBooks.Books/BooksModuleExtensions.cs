using Microsoft.Extensions.DependencyInjection;

namespace RiverBooks.Books;

public static class BooksModuleExtensions
{
    public static IServiceCollection AddBooksModuleServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        // Additional configuration as needed
        return services;
    }
}

