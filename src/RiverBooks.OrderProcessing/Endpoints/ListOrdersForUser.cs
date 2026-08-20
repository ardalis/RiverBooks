using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using Mediator;
using RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.OrderProcessing.Endpoints;
internal class ListOrdersForUser :
  EndpointWithoutRequest<ListOrdersForUserResponse>
{
  private readonly IMediator _mediator;

  public ListOrdersForUser(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/orders");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new ListOrdersForUserQuery(emailAddress!);

    var result = await _mediator.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await HttpContext.Response.SendUnauthorizedAsync();
    }
    else
    {
      var response = new ListOrdersForUserResponse();
      response.Orders = result.Value
        .Select(o =>
          new OrderSummary()
          {
            OrderId = o.OrderId,
            DateCreated = o.DateCreated,
            Total = o.Total,
            UserId = o.UserId
          })
        .ToList();
      await HttpContext.Response.SendAsync(response);
    }
  }
}
