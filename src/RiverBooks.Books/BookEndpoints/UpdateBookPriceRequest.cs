namespace RiverBooks.Books.BookEndpoints;

public record UpdateBookPriceRequest(Guid Id, decimal NewPrice);
