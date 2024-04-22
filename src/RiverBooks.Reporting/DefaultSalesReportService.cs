using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RiverBooks.Reporting;

public class DefaultSalesReportService : ISalesReportService
{
  private readonly ILogger<DefaultSalesReportService> _logger;
  private readonly string _connString;

  public DefaultSalesReportService(IConfiguration config,
    ILogger<DefaultSalesReportService> logger)
  {
    _connString = config.GetConnectionString("ReportingConnectionString")!;
    _logger = logger;
  }

  public async Task<TopBooksByMonthReport> GetTopBooksByMonthReport(int month, int year)
  {
    string sql = @"
select BookId, Title, Author, UnitsSold as Units, TotalSales as Sales
from Reporting.MonthlyBookSales
where Month = @month and Year = @year
ORDER BY TotalSales DESC
";
    using var conn = new SqlConnection(_connString);
    _logger.LogInformation("Executing query: {sql}", sql);
    var results = (await conn.QueryAsync<BookSalesResult>(sql, new { month, year }))
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
