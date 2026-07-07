using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Data;
using Serilog;

namespace RiverBooks.Books;

public static class BookModuleServiceExtensions
{
  public static IServiceCollection AddBookModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> moduleAssemblies)
  {
    string? connectionString = config.GetConnectionString("BooksConnectionString");
    services.AddDbContext<BookDbContext>(config =>
      config.UseSqlServer(connectionString));
    services.AddScoped<IBookRepository, EfBookRepository>();
    services.AddScoped<IBookService, BookService>();

    // if using Mediator in this module, add any assemblies that contain handlers to the list
    moduleAssemblies.Add(typeof(BookModuleServiceExtensions).Assembly);

    logger.Information("{Module} module services registered", "Books");
    return services;
  }
}
