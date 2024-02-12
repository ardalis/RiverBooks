namespace RiverBooks.Users.CartEndpoints;

public record CheckoutRequest(Guid ShippingAddressId, Guid BillingAddressId);
