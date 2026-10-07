using Ardalis.Result;
using Mediator;

namespace RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;

internal record ListOrdersForUserQuery(string EmailAddress) : IRequest<Result<List<OrderSummary>>>;
