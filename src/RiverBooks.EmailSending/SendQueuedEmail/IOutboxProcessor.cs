namespace RiverBooks.EmailSending.SendQueuedEmail;

public interface IOutboxProcessor
{
  Task CheckForEmailsToSend();
}
