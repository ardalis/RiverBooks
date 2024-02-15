namespace RiverBooks.EmailSending;

public interface IOutboxProcessor
{
  Task CheckForEmailsToSend();
}
