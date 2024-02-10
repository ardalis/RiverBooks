using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases;

namespace RiverBooks.Users.CartEndpoints;
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
      await SendUnauthorizedAsync();
    }
    else
    {
      var response = new ListOrdersForUserResponse();
      response.Orders = response.Orders
        .Select(o => 
          new OrderSummary()
          {
            DateCreated = o.DateCreated,
            DateShipped = o.DateShipped,
            Total = o.Total,
            UserId = o.UserId
          })
        .ToList();
      await SendAsync(response);
    }
  }
}
