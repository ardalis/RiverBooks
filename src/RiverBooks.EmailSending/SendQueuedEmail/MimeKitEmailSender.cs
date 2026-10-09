using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RiverBooks.EmailSending.SendQueuedEmail;

internal class MimeKitEmailSender : ISendEmail
{
  private readonly ILogger<MimeKitEmailSender> _logger;
  private readonly EmailSettings _emailSettings;

  public MimeKitEmailSender(ILogger<MimeKitEmailSender> logger,
    IOptions<EmailSettings> emailSettings)
  {
    _logger = logger;
    _emailSettings = emailSettings.Value;
  }

  public async Task SendEmailAsync(string to, string from, string subject, string body)
  {
    _logger.LogInformation("Attempting to send email to {to} from {from} with subject {subject}...", to, from, subject);

    using (var client = new SmtpClient()) // use localhost and a test server
    {
      client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, false);
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(from, from));
      message.To.Add(new MailboxAddress(to, to));
      message.Subject = subject;
      message.Body = new TextPart("plain") { Text = body };

      await client.SendAsync(message);
      _logger.LogInformation("Email sent!");

      client.Disconnect(true);
    }
  }
}


