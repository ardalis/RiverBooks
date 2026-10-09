namespace RiverBooks.Reporting;

internal interface ISalesReportService
{
  Task<TopBooksByMonthReport> GetTopBooksByMonthReport(int month, int year);
}
