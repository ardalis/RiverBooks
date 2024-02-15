using Ardalis.Result;
using MediatR;
using MongoDB.Driver;
using RiverBooks.EmailSending.Contracts;

namespace RiverBooks.EmailSending.Integrations;
internal class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result<Guid>>
{
  private readonly IMongoCollection<EmailOutboxEntity> _emailEntityCollection;

  public SendEmailCommandHandler(IMongoCollection<EmailOutboxEntity> emailEntityCollection)
  {
    _emailEntityCollection = emailEntityCollection;
  }

  public async Task<Result<Guid>> Handle(SendEmailCommand request, CancellationToken ct)
  {
    // we're just storing in the outbox and returning the generated id
    Guid id = Guid.NewGuid();

    var emailEntity = new EmailOutboxEntity
    {
      Id = id,
      To = request.To,
      From = request.From,
      Subject = request.Subject,
      Body = request.Body
    };
    await _emailEntityCollection.InsertOneAsync(emailEntity);

    return id;
  }
}
