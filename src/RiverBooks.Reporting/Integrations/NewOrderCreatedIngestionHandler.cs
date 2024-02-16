using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Contracts;
using static System.Net.Mime.MediaTypeNames;

namespace RiverBooks.Reporting.Integrations;
internal class NewOrderCreatedIngestionHandler : INotificationHandler<OrderCreatedIntegrationEvent>
{
  private readonly ILogger<NewOrderCreatedIngestionHandler> _logger;

  public NewOrderCreatedIngestionHandler(ILogger<NewOrderCreatedIngestionHandler> logger)
  {
    _logger = logger;
  }

  public Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling order created event to populate reporting database...");

    return Task.CompletedTask;
  }
}

public class OrderIngestionService
{
  private readonly ILogger<OrderIngestionService> _logger;
  private readonly string _connString;

  public OrderIngestionService(IConfiguration config,
    ILogger<OrderIngestionService> logger)
  {
    _connString = config.GetConnectionString("OrderProcessingConnectionString")!;
    _logger = logger;
  }

  public async Task CreateTableAsync()
  {
    string sql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyBookSales' AND type = 'U')
BEGIN
    CREATE TABLE MonthlyBookSales
    (
        BookId INT,
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
  }

  public class BookSale
  {
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int UnitsSold { get; set; }
    public decimal TotalSales { get; set; }
  }

  public async Task AddOrUpdateMonthlyBookSaleAsync(SqlConnection conn, BookSale sale)
  {
    var sql = @"
    IF EXISTS (SELECT 1 FROM MonthlyBookSales WHERE BookId = @BookId AND Year = @Year AND Month = @Month)
    BEGIN
        -- Update existing record
        UPDATE MonthlyBookSales
        SET UnitsSold = UnitsSold + @UnitsSold, TotalSales = TotalSales + @TotalSales
        WHERE BookId = @BookId AND Year = @Year AND Month = @Month
    END
    ELSE
    BEGIN
        -- Insert new record
        INSERT INTO MonthlyBookSales (BookId, Title, Author, Year, Month, UnitsSold, TotalSales)
        VALUES (@BookId, @Title, @Author, @Year, @Month, @UnitsSold, @TotalSales)
    END";

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
