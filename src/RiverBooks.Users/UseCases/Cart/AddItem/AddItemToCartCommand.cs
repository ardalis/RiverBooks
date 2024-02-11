using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress) : IRequest<Result>;
