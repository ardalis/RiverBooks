using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.Data;
using RiverBooks.Users.UseCases;

namespace RiverBooks.Users.CartEndpoints;

internal class ListCartItems :
  EndpointWithoutRequest<CartResponse>
{
  private readonly IMediator _mediator;

  public ListCartItems(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/cart");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new ListCartItemsQuery(emailAddress!);

    var result = await _mediator.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await SendUnauthorizedAsync();
    }
    else
    {
      var response = new CartResponse();
      response.CartItems = result.Value;
      await SendAsync(response);
    }
  }
}
