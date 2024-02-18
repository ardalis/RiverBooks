using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public record CheckoutCartCommand(string EmailAddress, 
                                  Guid shippingAddressId, 
                                  Guid billingAddressId)
                                                          : IRequest<Result<Guid>>;
