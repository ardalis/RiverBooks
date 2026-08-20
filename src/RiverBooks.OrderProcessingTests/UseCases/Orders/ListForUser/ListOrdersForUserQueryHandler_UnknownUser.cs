using Ardalis.Result;
using Ardalis.Specification;
using Mediator;
using NSubstitute;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;
using RiverBooks.Users.Contracts;
using Shouldly;
using Xunit;

namespace RiverBooks.OrderProcessingTests.UseCases.Orders.ListForUser;

public class ListOrdersForUserQueryHandler_UnknownUser
{
  [Fact]
  public async Task ReturnsUnauthorized_WhenEmailAddressHasNoUser()
  {
    // Arrange
    var handler = new ListOrdersForUserQueryHandler(
      Substitute.For<IOrderRepository>(),
      MediatorReturningNotFound());

    // Act
    var result = await handler.Handle(
      new ListOrdersForUserQuery("nobody@example.com"), CancellationToken.None);

    // Assert
    result.Status.ShouldBe(ResultStatus.Unauthorized);
  }

  [Fact]
  public async Task DoesNotQueryOrders_WhenEmailAddressHasNoUser()
  {
    // Arrange
    var repository = Substitute.For<IOrderRepository>();
    var handler = new ListOrdersForUserQueryHandler(repository, MediatorReturningNotFound());

    // Act
    await handler.Handle(new ListOrdersForUserQuery("nobody@example.com"), CancellationToken.None);

    // Assert
    await repository.DidNotReceive().ListAsync(Arg.Any<ISpecification<Order>>());
    await repository.DidNotReceive().ListAsync();
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
