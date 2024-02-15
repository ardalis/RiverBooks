using MongoDB.Bson.Serialization.Attributes;

namespace RiverBooks.EmailSending;

[BsonIgnoreExtraElements]
public class EmailOutboxEntity
{
  public Guid Id { get; set; }
  public string To { get; set; } = string.Empty;
  public string From { get; set; } = string.Empty;
  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public DateTime? DateTimeUtcProcessed { get; set; } = null!;
}


