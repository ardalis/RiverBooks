using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;

namespace RiverBooks.OrderProcessing.Integrations;
internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDetailsResponse>>
{
  public Task<Result<OrderDetailsResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
  {
    // need to look up user addresses in materialized view

    // need to look up book details (via mediatR)

    // need to create order - use OrderFactory

    return Task.FromResult((Result<OrderDetailsResponse>)null!);
  }
}
