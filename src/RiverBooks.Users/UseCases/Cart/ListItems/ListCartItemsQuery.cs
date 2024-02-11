using Ardalis.Result;
using MediatR;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

internal record ListCartItemsQuery(string EmailAddress) : IRequest<Result<List<CartItemDto>>>;
