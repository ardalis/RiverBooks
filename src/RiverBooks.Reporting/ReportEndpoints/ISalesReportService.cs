namespace RiverBooks.Reporting.ReportEndpoints;

public interface ISalesReportService
{
  Task<TopBooksByMonthReport> GetTopBooksByMonthReport(int month, int year);
}

public class DefaultSalesReportService
{

}
