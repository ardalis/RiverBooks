using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Domain;

public class ApplicationUser : IdentityUser, IHaveDomainEvents
{
  public string FullName { get; set; } = string.Empty;

  private readonly List<CartItem> _cartItems = new();
  public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();

  private readonly List<UserStreetAddress> _addresses = new();
  public IReadOnlyCollection<UserStreetAddress> Addresses => _addresses.AsReadOnly();

  private List<DomainEventBase> _domainEvents = new();
  [NotMapped]
  public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
  void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();

  public void AddItemToCart(CartItem item)
  {
    Guard.Against.Null(item);

    var existingBook = _cartItems.SingleOrDefault(c => c.BookId == item.BookId);
    if (existingBook != null)
    {
      existingBook.AdjustQuantity(existingBook.Quantity + item.Quantity);

      existingBook.AdjustUnitPrice(item.UnitPrice);
      existingBook.UpdateDescription(item.Description);
      return;
    }
    _cartItems.Add(item);
  }

  internal UserStreetAddress AddAddress(Address address)
  {
    Guard.Against.Null(address);

    // find existing address and just return it
    var existingAddress = _addresses.SingleOrDefault(a => a.StreetAddress == address);
    if (existingAddress != null)
    {
      return existingAddress;
    }

    var newAddress = new UserStreetAddress(Id, address);
    _addresses.Add(newAddress);

    RegisterDomainEvent(new AddressAddedEvent(newAddress));
    return newAddress;
  }

  internal void ClearCart()
  {
    _cartItems.Clear();
  }
}
