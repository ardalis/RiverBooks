using System.Security.Claims;
using System.Text.Json;
using Ardalis.Result;
using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using RiverBooks.OrderProcessing.Endpoints;
using RiverBooks.OrderProcessing.UseCases;
using RiverBooks.OrderProcessing.UseCases.Orders.GetDetails;
using Shouldly;
using Xunit;

namespace RiverBooks.OrderProcessingTests.Endpoints;

public class GetOrderDetails_ReturnsQueryResult
{
  private const string EmailAddress = "reader@example.com";

  [Fact]
  public async Task SendsEmailClaimAndRouteOrderIdInQuery_WhenRequestIsHandled()
  {
    // Arrange
    var orderId = Guid.NewGuid();
    var mediator = MediatorReturning(Result.Success(OrderDetailsFixtures.Make()));
    var endpoint = CreateEndpoint(mediator, out _);

    // Act
    await endpoint.HandleAsync(new GetOrderDetailsRequest { OrderId = orderId }, CancellationToken.None);

    // Assert
    await mediator.Received(1).Send(
      Arg.Is<GetOrderDetailsQuery>(q => q.EmailAddress == EmailAddress && q.OrderId == orderId),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ReturnsOrderDetailsFromQueryResult_WhenQuerySucceeds()
  {
    // Arrange
    var details = OrderDetailsFixtures.Make();
    var endpoint = CreateEndpoint(MediatorReturning(Result.Success(details)), out var httpContext);

    // Act
    await endpoint.HandleAsync(new GetOrderDetailsRequest { OrderId = details.OrderId }, CancellationToken.None);

    // Assert
    httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    var response = ReadResponse(httpContext);
    response.OrderId.ShouldBe(details.OrderId);
    response.UserId.ShouldBe(details.UserId);
    response.DateCreated.ShouldBe(details.DateCreated);
    response.Total.ShouldBe(details.Total);
    response.ShippingAddress.ShouldBe(details.ShippingAddress);
    response.BillingAddress.ShouldBe(details.BillingAddress);
    response.Items.ShouldBe(details.Items);
  }

  [Fact]
  public async Task Returns404_WhenQueryReturnsNotFound()
  {
    // Arrange
    var endpoint = CreateEndpoint(MediatorReturning(Result<OrderDetails>.NotFound()), out var httpContext);

    // Act
    await endpoint.HandleAsync(new GetOrderDetailsRequest { OrderId = Guid.NewGuid() }, CancellationToken.None);

    // Assert
    httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
  }

  [Fact]
  public async Task Returns401_WhenQueryReturnsUnauthorized()
  {
    // Arrange
    var endpoint = CreateEndpoint(MediatorReturning(Result<OrderDetails>.Unauthorized()), out var httpContext);

    // Act
    await endpoint.HandleAsync(new GetOrderDetailsRequest { OrderId = Guid.NewGuid() }, CancellationToken.None);

    // Assert
    httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
  }

  private static IMediator MediatorReturning(Result<OrderDetails> queryResult)
  {
    var mediator = Substitute.For<IMediator>();
    mediator
      .Send(Arg.Any<GetOrderDetailsQuery>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.FromResult(queryResult));

    return mediator;
  }

  private static GetOrderDetails CreateEndpoint(IMediator mediator, out DefaultHttpContext httpContext)
  {
    httpContext = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(
        new ClaimsIdentity([new Claim("EmailAddress", EmailAddress)], "test"))
    };
    httpContext.Response.Body = new MemoryStream();

    return Factory.Create<GetOrderDetails>(httpContext, mediator);
  }

  private static OrderDetails ReadResponse(DefaultHttpContext httpContext)
  {
    var responseBody = httpContext.Response.Body;
    responseBody.Position = 0;

    return JsonSerializer.Deserialize<OrderDetails>(
      responseBody,
      new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
  }
}

file static class OrderDetailsFixtures
{
  public static OrderDetails Make()
  {
    var orderId = Guid.NewGuid();

    return new()
    {
      OrderId = orderId,
      UserId = Guid.NewGuid(),
      ShippingAddress = new AddressDetails("1 Ship St", "Apt 2", "Kent", "OH", "44240", "USA"),
      BillingAddress = new AddressDetails("9 Bill Rd", "", "Akron", "OH", "44308", "USA"),
      DateCreated = new DateTimeOffset(2025, 5, 22, 17, 31, 0, TimeSpan.Zero),
      Total = 35.00m,
      Items =
      [
        new OrderItemDetails
        {
          OrderId = orderId,
          BookId = Guid.NewGuid(),
          Quantity = 3,
          UnitPrice = 10.00m,
          Description = "Three copies"
        },
        new OrderItemDetails
        {
          OrderId = orderId,
          BookId = Guid.NewGuid(),
          Quantity = 1,
          UnitPrice = 5.00m,
          Description = "One copy"
        }
      ]
    };
  }
}
