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

public class ListOrdersForUserQueryHandler_KnownUser
{
  private const string EmailAddress = "reader@example.com";

  [Fact]
  public async Task LooksUpUserIdFromEmailAddress_WhenQueryIsHandled()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var mediator = MediatorReturning(new UserDetailsResponse(userId, EmailAddress));
    var handler = new ListOrdersForUserQueryHandler(RepositoryReturning(), mediator);

    // Act
    await handler.Handle(new ListOrdersForUserQuery(EmailAddress), CancellationToken.None);

    // Assert
    await mediator.Received(1).Send(
      Arg.Is<UserDetailsByEmailQuery>(q => q.EmailAddress == EmailAddress),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task PassesSpecificationMatchingOnlyThatUsersOrders_WhenQueryIsHandled()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var theirOrder = OrderFixtures.Make(Guid.NewGuid());
    var ourOrder = OrderFixtures.Make(userId);
    var repository = RepositoryReturning();
    var handler = new ListOrdersForUserQueryHandler(
      repository,
      MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

    // Act
    await handler.Handle(new ListOrdersForUserQuery(EmailAddress), CancellationToken.None);

    // Assert
    var spec = (ISpecification<Order>)repository.ReceivedCalls()
      .Single(call => call.GetMethodInfo().GetParameters().Length == 1)
      .GetArguments()[0]!;
    spec.Evaluate([ourOrder, theirOrder]).ShouldBe([ourOrder]);
  }

  [Fact]
  public async Task ReturnsSummaryPerOrder_WhenRepositoryReturnsOrders()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var handler = new ListOrdersForUserQueryHandler(
      RepositoryReturning(OrderFixtures.Make(userId), OrderFixtures.Make(userId)),
      MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

    // Act
    var result = await handler.Handle(new ListOrdersForUserQuery(EmailAddress), CancellationToken.None);

    // Assert
    result.Value.Count.ShouldBe(2);
  }

  [Fact]
  public async Task ReturnsOrderId_WhenRepositoryReturnsOrders()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = OrderFixtures.Make(userId);
    var handler = new ListOrdersForUserQueryHandler(
      RepositoryReturning(order),
      MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

    // Act
    var result = await handler.Handle(new ListOrdersForUserQuery(EmailAddress), CancellationToken.None);

    // Assert
    result.Value.Single().OrderId.ShouldBe(order.Id);
  }

  [Fact]
  public async Task MultipliesUnitPriceByQuantityInTotal_WhenOrderItemQuantityExceedsOne()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = OrderFixtures.Make(userId,
      new OrderItem(Guid.NewGuid(), 3, 10.00m, "Three copies"),
      new OrderItem(Guid.NewGuid(), 2, 1.50m, "Two copies"));
    var handler = new ListOrdersForUserQueryHandler(
      RepositoryReturning(order),
      MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

    // Act
    var result = await handler.Handle(new ListOrdersForUserQuery(EmailAddress), CancellationToken.None);

    // Assert
    result.Value.Single().Total.ShouldBe(33.00m);
  }

  private static IOrderRepository RepositoryReturning(params Order[] orders)
  {
    var repository = Substitute.For<IOrderRepository>();
    repository.ListAsync(Arg.Any<ISpecification<Order>>()).Returns([.. orders]);

    return repository;
  }

  private static IMediator MediatorReturning(UserDetailsResponse userDetails)
  {
    var mediator = Substitute.For<IMediator>();
    mediator
      .Send(Arg.Any<UserDetailsByEmailQuery>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.FromResult(Result.Success(userDetails)));

    return mediator;
  }
}

file static class OrderFixtures
{
  public static Order Make(Guid userId, params OrderItem[] orderItems)
  {
    var address = new Address("123 Main St", "", "Kent", "OH", "44240", "USA");
    OrderItem[] items = orderItems.Length > 0
      ? orderItems
      : [new OrderItem(Guid.NewGuid(), 1, 9.99m, "A book")];

    return Order.Factory.Create(userId, address, address, items);
  }
}
