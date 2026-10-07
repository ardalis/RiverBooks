using Ardalis.Result;
using Ardalis.Specification;
using Mediator;
using NSubstitute;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.OrderProcessing.UseCases;
using RiverBooks.OrderProcessing.UseCases.Orders.GetDetails;
using RiverBooks.Users.Contracts;
using Shouldly;
using Xunit;

namespace RiverBooks.OrderProcessingTests.UseCases.Orders.GetDetails;

public class GetOrderDetailsQueryHandler_KnownUser
{
  private const string EmailAddress = "reader@example.com";

  [Fact]
  public async Task LooksUpUserIdFromEmailAddress_WhenQueryIsHandled()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var mediator = MediatorReturning(new UserDetailsResponse(userId, EmailAddress));
    var handler = new GetOrderDetailsQueryHandler(RepositoryReturning(null), mediator);

    // Act
    await handler.Handle(new GetOrderDetailsQuery(EmailAddress, Guid.NewGuid()), CancellationToken.None);

    // Assert
    await mediator.Received(1).Send(
      Arg.Is<UserDetailsByEmailQuery>(q => q.EmailAddress == EmailAddress),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task PassesSpecificationMatchingOnlyRequestedOrderOfThatUser_WhenQueryIsHandled()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var requestedOrder = OrderFixtures.Make(userId);
    var otherOrderOfSameUser = OrderFixtures.Make(userId);
    var otherUsersOrder = OrderFixtures.Make(Guid.NewGuid());
    var repository = RepositoryReturning(null);
    var handler = new GetOrderDetailsQueryHandler(
      repository,
      MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

    // Act
    await handler.Handle(new GetOrderDetailsQuery(EmailAddress, requestedOrder.Id), CancellationToken.None);

    // Assert
    var spec = (ISpecification<Order>)repository.ReceivedCalls().Single().GetArguments()[0]!;
    spec.Evaluate([requestedOrder, otherOrderOfSameUser, otherUsersOrder]).ShouldBe([requestedOrder]);
  }

  [Fact]
  public async Task PassesSpecificationExcludingOrder_WhenOrderBelongsToAnotherUser()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var otherUsersOrder = OrderFixtures.Make(Guid.NewGuid());
    var repository = RepositoryReturning(null);
    var handler = new GetOrderDetailsQueryHandler(
      repository,
      MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

    // Act
    await handler.Handle(new GetOrderDetailsQuery(EmailAddress, otherUsersOrder.Id), CancellationToken.None);

    // Assert
    var spec = (ISpecification<Order>)repository.ReceivedCalls().Single().GetArguments()[0]!;
    spec.Evaluate([otherUsersOrder]).ShouldBeEmpty();
  }

  [Fact]
  public async Task ReturnsNotFound_WhenRepositoryFindsNoOrder()
  {
    // Arrange
    var handler = new GetOrderDetailsQueryHandler(
      RepositoryReturning(null),
      MediatorReturning(new UserDetailsResponse(Guid.NewGuid(), EmailAddress)));

    // Act
    var result = await handler.Handle(new GetOrderDetailsQuery(EmailAddress, Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.Status.ShouldBe(ResultStatus.NotFound);
  }

  [Fact]
  public async Task ReturnsOrderHeaderFields_WhenOrderIsFound()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = OrderFixtures.Make(userId);
    var handler = HandlerFindingOrder(order, userId);

    // Act
    var result = await handler.Handle(new GetOrderDetailsQuery(EmailAddress, order.Id), CancellationToken.None);

    // Assert
    result.Value.OrderId.ShouldBe(order.Id);
    result.Value.UserId.ShouldBe(userId);
    result.Value.DateCreated.ShouldBe(order.DateCreated);
  }

  [Fact]
  public async Task ReturnsShippingAndBillingAddresses_WhenOrderIsFound()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var shipping = new Address("1 Ship St", "Apt 2", "Kent", "OH", "44240", "USA");
    var billing = new Address("9 Bill Rd", "", "Akron", "OH", "44308", "USA");
    var order = Order.Factory.Create(userId, shipping, billing,
      [new OrderItem(Guid.NewGuid(), 1, 9.99m, "A book")]);
    var handler = HandlerFindingOrder(order, userId);

    // Act
    var result = await handler.Handle(new GetOrderDetailsQuery(EmailAddress, order.Id), CancellationToken.None);

    // Assert
    result.Value.ShippingAddress.ShouldBe(new AddressDetails("1 Ship St", "Apt 2", "Kent", "OH", "44240", "USA"));
    result.Value.BillingAddress.ShouldBe(new AddressDetails("9 Bill Rd", "", "Akron", "OH", "44308", "USA"));
  }

  [Fact]
  public async Task ReturnsItemDetailsPerOrderItem_WhenOrderIsFound()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var bookId = Guid.NewGuid();
    var order = OrderFixtures.Make(userId,
      new OrderItem(bookId, 3, 10.00m, "Three copies"),
      new OrderItem(Guid.NewGuid(), 1, 5.00m, "One copy"));
    var handler = HandlerFindingOrder(order, userId);

    // Act
    var result = await handler.Handle(new GetOrderDetailsQuery(EmailAddress, order.Id), CancellationToken.None);

    // Assert
    result.Value.Items.Count.ShouldBe(2);
    result.Value.Items[0].ShouldBe(new OrderItemDetails
    {
      OrderId = order.Id,
      BookId = bookId,
      Quantity = 3,
      UnitPrice = 10.00m,
      Description = "Three copies"
    });
  }

  [Fact]
  public async Task MultipliesUnitPriceByQuantityInTotal_WhenOrderItemQuantityExceedsOne()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = OrderFixtures.Make(userId,
      new OrderItem(Guid.NewGuid(), 3, 10.00m, "Three copies"),
      new OrderItem(Guid.NewGuid(), 2, 1.50m, "Two copies"));
    var handler = HandlerFindingOrder(order, userId);

    // Act
    var result = await handler.Handle(new GetOrderDetailsQuery(EmailAddress, order.Id), CancellationToken.None);

    // Assert
    result.Value.Total.ShouldBe(33.00m);
  }

  private static GetOrderDetailsQueryHandler HandlerFindingOrder(Order order, Guid userId) =>
    new(RepositoryReturning(order), MediatorReturning(new UserDetailsResponse(userId, EmailAddress)));

  private static IOrderRepository RepositoryReturning(Order? order)
  {
    var repository = Substitute.For<IOrderRepository>();
    repository.FirstOrDefaultAsync(Arg.Any<ISingleResultSpecification<Order>>()).Returns(order);

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
