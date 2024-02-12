using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using RiverBooks.Users.UseCases.User.ListAddresses;

namespace RiverBooks.Users.UserEndpoints;
internal class ListAddresses :
  EndpointWithoutRequest<AddressListResponse>
{
  private readonly IMediator _mediator;

  public ListAddresses(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/users/addresses");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new ListAddressesQuery(emailAddress!);

    var result = await _mediator.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await SendUnauthorizedAsync();
    }
    else
    {
      var response = new AddressListResponse();

      response.Addresses = result.Value;

      await SendAsync(response);
    }
  }
}
