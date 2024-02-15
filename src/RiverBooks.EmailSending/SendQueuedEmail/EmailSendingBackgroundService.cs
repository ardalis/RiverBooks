using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RiverBooks.EmailSending.SendQueuedEmail;

public class EmailSendingBackgroundService : BackgroundService
{
  private readonly ILogger<EmailSendingBackgroundService> _logger;
  private readonly IOutboxProcessor _outboxProcessor;

  public EmailSendingBackgroundService(ILogger<EmailSendingBackgroundService> logger,
      IOutboxProcessor outboxProcessor)
  {
    _logger = logger;
    _outboxProcessor = outboxProcessor;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var delayMilliseconds = 30_000; // 30 seconds
    _logger.LogInformation("{serviceName} starting.", nameof(EmailSendingBackgroundService));
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await _outboxProcessor.CheckForEmailsToSend();
      }
      catch (Exception ex)
      {
        _logger.LogError("Error processing outbox: {message}", ex.Message);
      }
      finally
      {
        await Task.Delay(delayMilliseconds, stoppingToken);
      }
    }
    _logger.LogInformation("{serviceName} stopping.", nameof(EmailSendingBackgroundService));
  }
}
