using Ardalis.Result;
using Mediator;

namespace RiverBooks.OrderProcessing.UseCases.Orders.GetDetails;

internal record GetOrderDetailsQuery(string EmailAddress, Guid OrderId) : IRequest<Result<OrderDetails>>;
