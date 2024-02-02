using Ardalis.Result;
using MediatR;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.UseCases;

internal record ListCartItemsQuery(string EmailAddress) : IRequest<Result<List<CartItemDto>>>;
