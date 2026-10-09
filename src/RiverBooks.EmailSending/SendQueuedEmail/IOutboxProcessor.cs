namespace RiverBooks.EmailSending.SendQueuedEmail;

internal interface IOutboxProcessor
{
  Task CheckForEmailsToSend();
}
