namespace RiverBooks.Books.BookEndpoints;

internal record UpdateBookPriceRequest(Guid Id, decimal NewPrice);
