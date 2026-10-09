namespace RiverBooks.EmailSending.SendQueuedEmail;

internal interface ISendEmail
{
  Task SendEmailAsync(string to, string from, string subject, string body);
}
