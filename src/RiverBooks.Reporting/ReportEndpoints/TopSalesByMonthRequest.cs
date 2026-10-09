using Microsoft.AspNetCore.Mvc;

namespace RiverBooks.Reporting.ReportEndpoints;

internal class TopSalesByMonthRequest
{
  [FromQuery]
  public int Month { get; set; }
  [FromQuery]
  public int Year { get; set; }
}
