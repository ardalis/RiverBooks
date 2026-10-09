using System.ComponentModel.DataAnnotations;

namespace RiverBooks.EmailSending;

internal class EmailSettings
{
  [Required]
  public required string SmtpServer { get; set; }
  
  [Range(1, 65535)]
  public int SmtpPort { get; set; } = 25;
}
