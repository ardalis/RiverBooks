using FastEndpoints;

namespace RiverBooks.Reporting.ReportEndpoints;

internal class TopSalesByMonth2 :
  Endpoint<TopSalesByMonthRequest, TopSalesByMonthResponse>
{
  private readonly ISalesReportService _reportService;

  public TopSalesByMonth2(ISalesReportService reportService)
  {
    _reportService = reportService;
  }

  public override void Configure()
  {
    Get("/topsales2");
    AllowAnonymous(); // TODO: lock down
  }

  public override async Task HandleAsync(
    TopSalesByMonthRequest request,
    CancellationToken ct = default)
  {
    var report = await _reportService.GetTopBooksByMonthReport(request.Month, request.Year);
    var response = new TopSalesByMonthResponse { Report = report };
    await SendAsync(response);
  }
}
