namespace RiverBooks.Reporting;

public interface ISalesReportService
{
  Task<TopBooksByMonthReport> GetTopBooksByMonthReport(int month, int year);
}
