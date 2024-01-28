using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Books;

internal class BookDbContext : DbContext
{
  internal DbSet<Book> Books { get; set; }

  public BookDbContext(DbContextOptions options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("Books");
  }
}
