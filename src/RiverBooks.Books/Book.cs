using Ardalis.GuardClauses;

namespace RiverBooks.Books;

internal class Book
{
  public Guid Id { get; private set; } = Guid.NewGuid();
  public string Title { get; private set; } = string.Empty;
  public string Author { get; private set; } = string.Empty;
  public decimal Price { get; private set; }

  internal Book(Guid id, string title, string author, decimal price)
  {
    Id = Guard.Against.Default(id);
    Title = Guard.Against.NullOrEmpty(title);
    Author = Guard.Against.NullOrEmpty(author);
    Price = Guard.Against.Negative(price);
  }

  internal void UpdateTitle(string newTitle)
  {
    Title = Guard.Against.NullOrEmpty(newTitle);
  }

  internal void UpdateAuthor(string newAuthor)
  {
    Author = Guard.Against.NullOrEmpty(newAuthor);
  }

  internal void UpdatePrice(decimal newPrice)
  {
    Price = Guard.Against.Negative(newPrice);
  }
}
