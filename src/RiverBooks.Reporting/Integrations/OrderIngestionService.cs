using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Reporting.Integrations;

public class OrderIngestionService
{
  private readonly ILogger<OrderIngestionService> _logger;
  private readonly string _connString;
  private static bool _ensureTableCreated = false;

  public OrderIngestionService(IConfiguration config,
    ILogger<OrderIngestionService> logger)
  {
    _connString = config.GetConnectionString("ReportingConnectionString")!;
    _logger = logger;
  }

  private async Task CreateTableAsync()
  {
    string sql = @"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Reporting')
BEGIN
    EXEC('CREATE SCHEMA Reporting')
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyBookSales' AND type = 'U')
BEGIN
    CREATE TABLE Reporting.MonthlyBookSales
    (
        BookId uniqueidentifier,
        Title NVARCHAR(255),
        Author NVARCHAR(255),
        Year INT,
        Month INT,
        UnitsSold INT,
        TotalSales DECIMAL(18, 2),
        PRIMARY KEY (BookId, Year, Month)
    );
END

";
    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);
    
    await conn.ExecuteAsync(sql);
    _ensureTableCreated = true;
  }


  public async Task AddOrUpdateMonthlyBookSalesAsync(BookSale sale)
  {
    if (!_ensureTableCreated) await CreateTableAsync();

    var sql = @"
    IF EXISTS (SELECT 1 FROM Reporting.MonthlyBookSales WHERE BookId = @BookId AND Year = @Year AND Month = @Month)
    BEGIN
        -- Update existing record
        UPDATE Reporting.MonthlyBookSales
        SET UnitsSold = UnitsSold + @UnitsSold, TotalSales = TotalSales + @TotalSales
        WHERE BookId = @BookId AND Year = @Year AND Month = @Month
    END
    ELSE
    BEGIN
        -- Insert new record
        INSERT INTO Reporting.MonthlyBookSales (BookId, Title, Author, Year, Month, UnitsSold, TotalSales)
        VALUES (@BookId, @Title, @Author, @Year, @Month, @UnitsSold, @TotalSales)
    END";

    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);
    await conn.ExecuteAsync(sql, new
    {
      sale.BookId,
      sale.Title,
      sale.Author,
      sale.Year,
      sale.Month,
      sale.UnitsSold,
      sale.TotalSales
    });
  }
}
