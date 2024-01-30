namespace RiverBooks.Users.CartEndpoints;

public record CartItemDto(Guid ItemId, Guid BookId, string BookTitle, int Quantity, decimal UnitPrice);
