using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RiverBooks.Books.Data;
using Testcontainers.MsSql;

namespace RiverBooks.Books.Tests.Endpoints;

public class Fixture
  : AppFixture<Program>
{
  private const string SqlPassword = "yourStrong(!)Password";
  private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";
  private const int MaxMigrationAttempts = 5;

  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder(SqlServerImage)
    .WithPassword(SqlPassword)
    .Build();
  private readonly SemaphoreSlim _setupLock = new(1, 1);
  private readonly string _databaseName = $"RiverBooksTests_{Guid.NewGuid():N}";

  private string? _booksConnectionString;
  private bool _databaseMigrated;

  protected override async ValueTask PreSetupAsync()
  {
    await _dbContainer.StartAsync();
    await WaitForSqlServerAsync(_dbContainer.GetConnectionString());

    var connectionStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_dbContainer.GetConnectionString())
    {
      InitialCatalog = _databaseName,
      TrustServerCertificate = true
    };

    _booksConnectionString = connectionStringBuilder.ConnectionString;
  }

  protected override void ConfigureApp(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      services.RemoveAll<DbContextOptions<BookDbContext>>();

      services.AddDbContext<BookDbContext>(options =>
        options.UseSqlServer(_booksConnectionString!));
    });
  }

  protected override async ValueTask SetupAsync()
  {
    if (_databaseMigrated)
    {
      return;
    }

    await _setupLock.WaitAsync();
    try
    {
      if (_databaseMigrated)
      {
        return;
      }

      using var scope = Services.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();

      await MigrateWithRetryAsync(dbContext);
      _databaseMigrated = true;
    }
    finally
    {
      _setupLock.Release();
    }
  }

  protected override async ValueTask TearDownAsync()
  {
    await _dbContainer.DisposeAsync();
  }

  private static async Task WaitForSqlServerAsync(string connectionString)
  {
    SqlException? lastException = null;

    for (var attempt = 1; attempt <= MaxMigrationAttempts; attempt++)
    {
      try
      {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return;
      }
      catch (SqlException ex) when (attempt < MaxMigrationAttempts)
      {
        lastException = ex;
        await Task.Delay(TimeSpan.FromSeconds(attempt));
      }
      catch (SqlException ex)
      {
        lastException = ex;
      }
    }

    if (lastException is not null)
    {
      throw lastException;
    }

    throw new InvalidOperationException("Could not connect to SQL Server.");
  }

  private static async Task MigrateWithRetryAsync(BookDbContext dbContext)
  {
    SqlException? lastException = null;

    for (var attempt = 1; attempt <= MaxMigrationAttempts; attempt++)
    {
      try
      {
        await dbContext.Database.MigrateAsync();
        return;
      }
      catch (SqlException ex) when (attempt < MaxMigrationAttempts && IsRetriable(ex))
      {
        lastException = ex;
        await Task.Delay(TimeSpan.FromSeconds(attempt));
      }
      catch (SqlException ex)
      {
        lastException = ex;
        break;
      }
    }

    if (lastException is not null)
    {
      throw lastException;
    }

    throw new InvalidOperationException("Could not migrate test database.");
  }

  private static bool IsRetriable(SqlException ex)
  {
    return ex.Number is 1801 or 53 or 64 or 233 or 4060
      || ex.Message.Contains("transport-level error", StringComparison.OrdinalIgnoreCase)
      || ex.Message.Contains("network-related", StringComparison.OrdinalIgnoreCase);
  }
}
