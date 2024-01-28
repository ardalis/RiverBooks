namespace RiverBooks.Books;

public record UpdateBookPriceRequest(Guid Id, decimal NewPrice);
