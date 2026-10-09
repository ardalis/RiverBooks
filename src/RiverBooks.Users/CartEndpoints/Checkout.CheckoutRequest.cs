namespace RiverBooks.Users.CartEndpoints;

internal record CheckoutRequest(Guid ShippingAddressId, Guid BillingAddressId);
