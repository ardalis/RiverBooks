namespace RiverBooks.Reporting.Integrations;

public class BookSale
{
  public Guid BookId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Author { get; set; } = string.Empty;
  public int Year { get; set; }
  public int Month { get; set; }
  public int UnitsSold { get; set; }
  public decimal TotalSales { get; set; }
}
