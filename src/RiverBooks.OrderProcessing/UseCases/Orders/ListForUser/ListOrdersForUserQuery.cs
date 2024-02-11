using Ardalis.Result;
using MediatR;
using RiverBooks.Users.UseCases;

namespace RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;

internal record ListOrdersForUserQuery(string EmailAddress) : IRequest<Result<List<OrderSummary>>>;
