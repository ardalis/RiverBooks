
namespace RiverBooks.EmailSending.EmailEndpoints;

public class ListEmailsResponse
{
  public int Count { get; set; }
  public List<EmailOutboxEntity> Emails { get; internal set; } = new();
}

