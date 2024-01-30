using Ardalis.GuardClauses;

namespace RiverBooks.Users;

public class CartItem
{
  public CartItem(Guid bookId, int quantity, decimal unitPrice)
  {
    BookId = Guard.Against.Default(bookId);
    Quantity = Guard.Against.Negative(quantity);
    UnitPrice = Guard.Against.Negative(unitPrice);
  }

  public Guid Id { get; private set; } = Guid.NewGuid();
  public Guid BookId { get; private set; }
  public int Quantity { get; private set; }
  public decimal UnitPrice { get; private set; }

  public void AdjustQuantity(int quantity)
  {
    Quantity = Guard.Against.Negative(quantity);
  }
}
