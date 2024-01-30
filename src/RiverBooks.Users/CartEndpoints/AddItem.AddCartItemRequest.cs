namespace RiverBooks.Users.CartEndpoints;

internal record AddCartItemRequest(Guid BookId, int Quantity);
