using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases;

public record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress) : IRequest<Result>;
