namespace RiverBooks.Users.CartEndpoints;

internal class CartResponse
{
  public List<CartItemDto> CartItems { get; set; } = new();
}
