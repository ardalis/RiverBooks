namespace RiverBooks.EmailSending.SendQueuedEmail;

public interface ISendEmail
{
  Task SendEmailAsync(string to, string from, string subject, string body);
}
