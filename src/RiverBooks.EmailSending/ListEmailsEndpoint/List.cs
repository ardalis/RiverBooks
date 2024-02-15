using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.ListEmailsEndpoint;
internal class List :
  EndpointWithoutRequest<ListEmailsResponse>
{
  private readonly IMongoCollection<EmailOutboxEntity> _emailEntityCollection;

  public List(IMongoCollection<EmailOutboxEntity> emailEntityCollection)
  {
    _emailEntityCollection = emailEntityCollection;
  }

  public override void Configure()
  {
    Get("/emails");
    AllowAnonymous(); // TODO:  Lock this down
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    // TODO: Implement paging
    var filter = Builders<EmailOutboxEntity>.Filter.Empty;
    var emailEntities = await _emailEntityCollection.Find(filter).ToListAsync();

    var response = new ListEmailsResponse()
    {
      Count = emailEntities.Count,
      Emails = emailEntities // TODO: Use a separate DTO
    };

    Response = response;
  }
}
