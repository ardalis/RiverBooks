using Ardalis.Result.AspNetCore;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.Domain;
using RiverBooks.Users.UseCases.Cart.AddItem;
using RiverBooks.Users.UseCases.User.Create;

namespace RiverBooks.Users.UserEndpoints;

internal sealed class Create : Endpoint<CreateUserRequest>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMediator _mediator;

  public Create(UserManager<ApplicationUser> userManager,
    IMediator mediator)
  {
    _userManager = userManager;
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/users");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateUserRequest req, 
    CancellationToken ct)
  {
    var command = new CreateUserCommand(req.Email, req.Password);

    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
      await SendResultAsync(result.ToMinimalApiResult());
      return;
    }
    await SendOkAsync();
  }
}
