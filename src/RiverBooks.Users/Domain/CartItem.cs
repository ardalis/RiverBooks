using Ardalis.GuardClauses;

namespace RiverBooks.Users.Domain;

public class CartItem
{
  public CartItem(Guid bookId, int quantity, decimal unitPrice, string description)
  {
    BookId = Guard.Against.Default(bookId);
    Quantity = Guard.Against.Negative(quantity);
    UnitPrice = Guard.Against.Negative(unitPrice);
    Description = Guard.Against.NullOrEmpty(description);
  }

  private CartItem()
  {
    // EF
  }

  public Guid Id { get; private set; } = Guid.NewGuid();
  public Guid BookId { get; private set; }
  public int Quantity { get; private set; }
  public string Description { get; private set; } = string.Empty;
  public decimal UnitPrice { get; private set; }

  public void AdjustQuantity(int quantity)
  {
    Quantity = Guard.Against.Negative(quantity);
  }

  public void AdjustUnitPrice(decimal unitPrice)
  {
    UnitPrice = Guard.Against.Negative(unitPrice);
  }

  public void UpdateDescription(string newDescription)
  {
    Description = Guard.Against.NullOrEmpty(newDescription);
  }
}
