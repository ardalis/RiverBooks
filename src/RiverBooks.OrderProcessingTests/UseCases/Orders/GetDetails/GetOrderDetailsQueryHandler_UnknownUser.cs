using Ardalis.Result;
using Ardalis.Specification;
using Mediator;
using NSubstitute;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.OrderProcessing.UseCases.Orders.GetDetails;
using RiverBooks.Users.Contracts;
using Shouldly;
using Xunit;

namespace RiverBooks.OrderProcessingTests.UseCases.Orders.GetDetails;

public class GetOrderDetailsQueryHandler_UnknownUser
{
  [Fact]
  public async Task ReturnsUnauthorized_WhenEmailAddressHasNoUser()
  {
    // Arrange
    var handler = new GetOrderDetailsQueryHandler(
      Substitute.For<IOrderRepository>(),
      MediatorReturningNotFound());

    // Act
    var result = await handler.Handle(
      new GetOrderDetailsQuery("nobody@example.com", Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.Status.ShouldBe(ResultStatus.Unauthorized);
  }

  [Fact]
  public async Task DoesNotQueryOrders_WhenEmailAddressHasNoUser()
  {
    // Arrange
    var repository = Substitute.For<IOrderRepository>();
    var handler = new GetOrderDetailsQueryHandler(repository, MediatorReturningNotFound());

    // Act
    await handler.Handle(new GetOrderDetailsQuery("nobody@example.com", Guid.NewGuid()), CancellationToken.None);

    // Assert
    await repository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<ISingleResultSpecification<Order>>());
  }

  private static IMediator MediatorReturningNotFound()
  {
    var mediator = Substitute.For<IMediator>();
    mediator
      .Send(Arg.Any<UserDetailsByEmailQuery>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.FromResult(Result<UserDetailsResponse>.NotFound()));

    return mediator;
  }
}
