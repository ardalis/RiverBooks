using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.AddAddress;

internal class AddAddressToUserHandler : IRequestHandler<AddAddressToUserCommand, Result>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly IMediator _mediator;
  private readonly ILogger<AddAddressToUserHandler> _logger;

  public AddAddressToUserHandler(IApplicationUserRepository userRepository,
    IMediator mediator,
    ILogger<AddAddressToUserHandler> logger)
  {
    _userRepository = userRepository;
    _mediator = mediator;
    _logger = logger;
  }

  public async Task<Result> Handle(AddAddressToUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetUserWithAddressesByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }

    var addressToAdd = new Address(request.Street1,
                                   request.Street2,
                                   request.City,
                                   request.State,
                                   request.PostalCode,
                                   request.Country);
    var userAddress = user.AddAddress(addressToAdd);
    await _userRepository.SaveChangesAsync();

    _logger.LogInformation("Added address {address} to user {email}", userAddress, request.EmailAddress);

    return Result.Success();
  }

}
