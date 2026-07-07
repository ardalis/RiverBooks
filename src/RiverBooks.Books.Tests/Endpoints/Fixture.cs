using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Data;
using Testcontainers.MsSql;

namespace RiverBooks.Books.Tests.Endpoints;

public class Fixture
  : AppFixture<Program>
{
  private const string DatabaseName = "RiverBooksTests";
  private const string SqlPassword = "yourStrong(!)Password";
  private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";

  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder(SqlServerImage)
    .WithPassword(SqlPassword)
    .Build();

  private string? _booksConnectionString;

  protected override async ValueTask PreSetupAsync()
  {
    await _dbContainer.StartAsync();

    var connectionStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_dbContainer.GetConnectionString())
    {
      InitialCatalog = DatabaseName,
      TrustServerCertificate = true
    };

    _booksConnectionString = connectionStringBuilder.ConnectionString;
  }

  protected override void ConfigureApp(IWebHostBuilder builder)
  {
    builder.ConfigureAppConfiguration((_, config) =>
    {
      config.AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["ConnectionStrings:BooksConnectionString"] = _booksConnectionString
      });
    });
  }

  protected override async ValueTask SetupAsync()
  {
    using var scope = Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();

    await dbContext.Database.MigrateAsync();
  }

  protected override async ValueTask TearDownAsync()
  {
    await _dbContainer.DisposeAsync();
  }
}
