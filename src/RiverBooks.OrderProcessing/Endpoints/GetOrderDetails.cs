using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using Mediator;
using RiverBooks.OrderProcessing.UseCases;
using RiverBooks.OrderProcessing.UseCases.Orders.GetDetails;

namespace RiverBooks.OrderProcessing.Endpoints;

internal class GetOrderDetails :
  Endpoint<GetOrderDetailsRequest, OrderDetails>
{
  private readonly IMediator _mediator;

  public GetOrderDetails(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/orders/{OrderId}");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(GetOrderDetailsRequest request,
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new GetOrderDetailsQuery(emailAddress!, request.OrderId);

    var result = await _mediator.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await HttpContext.Response.SendUnauthorizedAsync();
    }
    else if (result.Status == ResultStatus.NotFound)
    {
      await HttpContext.Response.SendNotFoundAsync(ct);
    }
    else
    {
      await HttpContext.Response.SendAsync(result.Value);
    }
  }
}
