namespace RiverBooks.Reporting;

internal interface ITopSellingBooksReportService
{
  TopBooksByMonthReport ReachInSqlQuery(int month, int year);
}