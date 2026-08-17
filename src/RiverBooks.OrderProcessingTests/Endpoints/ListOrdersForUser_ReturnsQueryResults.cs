using System.Security.Claims;
using System.Text.Json;
using Ardalis.Result;
using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using RiverBooks.OrderProcessing.Endpoints;
using RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;
using RiverBooks.Users.CartEndpoints;
using Shouldly;
using Xunit;
using OrderSummary = RiverBooks.Users.UseCases.OrderSummary;

namespace RiverBooks.OrderProcessingTests.Endpoints;

public class ListOrdersForUser_ReturnsQueryResults
{
  [Fact]
  public async Task ReturnsSummaryPerOrder_WhenQuerySucceeds()
  {
    // Arrange
    var summaries = new List<OrderSummary> { OrderSummaryFixtures.Make(), OrderSummaryFixtures.Make() };
    var endpoint = CreateEndpoint(Result.Success(summaries), out var responseBody);

    // Act
    await endpoint.HandleAsync(CancellationToken.None);

    // Assert
    ReadResponse(responseBody).Orders.Count.ShouldBe(2);
  }

  [Fact]
  public async Task ReturnsOrderFieldsFromQueryResult_WhenQuerySucceeds()
  {
    // Arrange
    var summary = OrderSummaryFixtures.Make();
    var endpoint = CreateEndpoint(Result.Success(new List<OrderSummary> { summary }), out var responseBody);

    // Act
    await endpoint.HandleAsync(CancellationToken.None);

    // Assert
    var order = ReadResponse(responseBody).Orders.Single();
    order.UserId.ShouldBe(summary.UserId);
    order.DateCreated.ShouldBe(summary.DateCreated);
    order.DateShipped.ShouldBe(summary.DateShipped);
    order.Total.ShouldBe(summary.Total);
  }

  private static ListOrdersForUser CreateEndpoint(
    Result<List<OrderSummary>> queryResult,
    out MemoryStream responseBody)
  {
    var mediator = Substitute.For<IMediator>();
    mediator
      .Send(Arg.Any<ListOrdersForUserQuery>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.FromResult(queryResult));

    responseBody = new MemoryStream();
    var httpContext = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(
        new ClaimsIdentity([new Claim("EmailAddress", "reader@example.com")], "test"))
    };
    httpContext.Response.Body = responseBody;

    return Factory.Create<ListOrdersForUser>(httpContext, mediator);
  }

  private static ListOrdersForUserResponse ReadResponse(MemoryStream responseBody)
  {
    responseBody.Position = 0;

    return JsonSerializer.Deserialize<ListOrdersForUserResponse>(
      responseBody,
      new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
  }
}

file static class OrderSummaryFixtures
{
  public static OrderSummary Make() =>
    new()
    {
      OrderId = Guid.NewGuid(),
      UserId = Guid.NewGuid(),
      DateCreated = new DateTimeOffset(2025, 5, 22, 17, 31, 0, TimeSpan.Zero),
      DateShipped = new DateTimeOffset(2025, 5, 24, 9, 0, 0, TimeSpan.Zero),
      Total = 42.50m
    };
}
