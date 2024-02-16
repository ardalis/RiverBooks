using System.Globalization;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Reporting;
internal class TopSellingBooksReportService : ITopSellingBooksReportService
{
  private readonly ILogger<TopSellingBooksReportService> _logger;
  private readonly string _connString;

  public TopSellingBooksReportService(IConfiguration config,
    ILogger<TopSellingBooksReportService> logger)
  {
    _connString = config.GetConnectionString("OrderProcessingConnectionString")!;
    _logger = logger;
  }

  public TopBooksByMonthReport ReachInSqlQuery(int month, int year)
  {
    string sql = @"
select b.Id, b.Title, b.Author, sum(oi.Quantity) as Units, sum(oi.UnitPrice * oi.Quantity) as Sales
from Books.Books b 
	inner join OrderProcessing.OrderItem oi on b.Id = oi.BookId
	inner join OrderProcessing.Orders o on o.Id = oi.OrderId
where MONTH(o.DateCreated) = @month and YEAR(o.DateCreated) = @year
group by b.Id, b.Title, b.Author
ORDER BY Sales DESC
";
    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);
    var results = conn.Query<BookSalesResult>(sql, new { month, year })
      .ToList();

    var report = new TopBooksByMonthReport
    {
      Year = year,
      Month = month,
      MonthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(month),
      Results = results
    };

    return report;
  }
}
