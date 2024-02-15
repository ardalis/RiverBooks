using System.Security.Claims;
using FastEndpoints;
using StackExchange.Redis;
using RiverBooks.OrderProcessing.UseCases.Orders.ListForUser;
using Microsoft.Extensions.Logging;

namespace RiverBooks.OrderProcessing.Endpoints;

/// <summary>
/// Used for testing only
/// </summary>
internal class FlushCache : EndpointWithoutRequest
{
  private readonly IDatabase _db;
  private readonly ILogger<FlushCache> _logger;

  public FlushCache(ILogger<FlushCache> logger)
  {
    // TODO: use DI
    var redis = ConnectionMultiplexer.Connect("localhost"); // TODO: Get server from config
    _db = redis.GetDatabase();
    _logger = logger;
  }

  public override void Configure()
  {
    Post("/flushcache");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    CancellationToken ct = default)
  {
    await _db.ExecuteAsync("FLUSHDB");
    _logger.LogWarning("FLUSHED CACHE FOR {db}", "REDIS");
  }
}
