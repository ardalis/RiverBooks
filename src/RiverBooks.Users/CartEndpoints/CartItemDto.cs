namespace RiverBooks.Users.CartEndpoints;

public record CartItemDto(Guid ItemId, Guid BookId, string Description, int Quantity, decimal UnitPrice);
