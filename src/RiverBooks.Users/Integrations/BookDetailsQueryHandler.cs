using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.GetByEmail;

namespace RiverBooks.Users.Integrations;

internal class UserDetailsByEmailQueryHandler : IRequestHandler<UserDetailsByEmailQuery, 
                                                        Result<UserDetailsResponse>>
{
  private readonly IMediator _mediator;

  public UserDetailsByEmailQueryHandler(IMediator mediator)
  {
    _mediator = mediator;
  }


  public async Task<Result<UserDetailsResponse>> Handle(UserDetailsByEmailQuery request, CancellationToken cancellationToken)
  {
    var query = new GetUserByEmailQuery(request.EmailAddress);

    var result = await _mediator.Send(query);

    if(result.Status != ResultStatus.Ok) { return Result.NotFound(); }

    var response = new UserDetailsResponse(result.Value.UserId, result.Value.EmailAddress);

    return response;
  }
}
