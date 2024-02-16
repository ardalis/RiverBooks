using FastEndpoints;

namespace RiverBooks.Reporting.ReportEndpoints;

internal class TopSalesByMonth1 :
  Endpoint<TopSalesByMonthRequest, TopSalesByMonthResponse>
{
  private readonly ITopSellingBooksReportService _reportService;

  public TopSalesByMonth1(ITopSellingBooksReportService reportService)
  {
    _reportService = reportService;
  }

  public override void Configure()
  {
    Get("/topsales");
    AllowAnonymous(); // TODO: lock down
  }

  public override async Task HandleAsync(
    TopSalesByMonthRequest request,
    CancellationToken ct = default)
  {
    var report = _reportService.ReachInSqlQuery(request.Month, request.Year);
    var response = new TopSalesByMonthResponse { Report = report };
    await SendAsync(response);
  }
}
