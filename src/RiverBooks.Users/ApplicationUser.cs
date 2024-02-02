using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users;

public class ApplicationUser : IdentityUser 
{
  public string FullName { get; set; } = string.Empty;

  private readonly List<CartItem> _cartItems = new();
  public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();

  public void AddItemToCart(CartItem item)
  {
    Guard.Against.Null(item);

    var existingBook = _cartItems.SingleOrDefault(c => c.BookId == item.BookId);
    if(existingBook != null)
    {
      existingBook.AdjustQuantity(existingBook.Quantity + item.Quantity);

      // TODO: What if price or title is different? Let's use the new values
      existingBook.AdjustUnitPrice(item.UnitPrice);
      existingBook.UpdateDescription(item.Description);
      return;
    }
    _cartItems.Add(item);
  }
}
