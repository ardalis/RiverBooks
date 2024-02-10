using Ardalis.GuardClauses;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderItem
{
  public OrderItem(Guid bookId, int quantity, decimal unitPrice, string description)
  {
    BookId = Guard.Against.Default(bookId);
    Quantity = Guard.Against.Negative(quantity);
    UnitPrice = Guard.Against.Negative(unitPrice);
    Description = Guard.Against.NullOrEmpty(description);
  }

  public Guid BookId { get; private set; }
  public int Quantity { get; private set; }
  public decimal UnitPrice { get; private set; }
  public object Description { get; private set; }
}



