using Ardalis.Result;
using MediatR;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public record CheckoutCartCommand(string EmailAddress, 
                                  Guid shippingAddressId, 
                                  Guid billingAddressId)
                                                          : IRequest<Result<Guid>>;
