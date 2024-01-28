namespace RiverBooks.Books;

public class CreateBookRequest
{
  public Guid? Id {  get; set; }
  public string Title { get; set; } = string.Empty;
  public string Author { get; set; } = string.Empty;
  public decimal Price { get; set; }
}
