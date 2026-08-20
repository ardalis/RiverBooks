using Ardalis.Result;
using Mediator;

namespace RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;

public record ListOrdersForUserQuery(string EmailAddress) : IRequest<Result<List<OrderSummary>>>;
