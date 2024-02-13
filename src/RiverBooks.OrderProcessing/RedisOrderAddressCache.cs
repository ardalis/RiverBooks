using System.Text.Json;
using Ardalis.Result;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using StackExchange.Redis;

namespace RiverBooks.OrderProcessing;

internal class RedisOrderAddressCache : IOrderAddressCache
{
  private readonly IDatabase _db;

  public RedisOrderAddressCache()
  {
    var redis = ConnectionMultiplexer.Connect("localhost");
    _db = redis.GetDatabase();
  }

  public async Task<Result<OrderAddress>> GetByIdAsync(Guid id)
  {
    string? fetchedJson = await _db.StringGetAsync(id.ToString());

    if (fetchedJson is null) return Result.NotFound();

    var address = JsonSerializer.Deserialize<OrderAddress>(fetchedJson);

    if (address is null) return Result.NotFound();

    return Result.Success(address);
  }

  public async Task<Result> StoreAsync(OrderAddress orderAddress)
  {
    var key = orderAddress.Id.ToString();
    var addressJson = JsonSerializer.Serialize(orderAddress);

    await _db.StringSetAsync(key, addressJson);

    return Result.Success();
  }
}
