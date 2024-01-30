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

    var existingItem = _cartItems.SingleOrDefault(c => c.Id == item.BookId);
    if(existingItem != null)
    {
      existingItem.AdjustQuantity(existingItem.Quantity + item.Quantity);

      // TODO: What if price is different?
      return;
    }
    _cartItems.Add(item);
  }
}
