namespace RiverBooks.Books.Tests.Entities;

public class BookUpdateTitle
{
  [Fact]
  public void ThrowsGivenEmptyTitle()
  {
    var book = new Book(Guid.NewGuid(), "title", "author", 1m);
    Assert.Throws<ArgumentException>(() => book.UpdateTitle(""));
  }

  [Fact]
  public void ThrowsGivenNullTitle()
  {
    var book = new Book(Guid.NewGuid(), "title", "author", 1m);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    Assert.Throws<ArgumentNullException>(() => book.UpdateTitle(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
  }
}
