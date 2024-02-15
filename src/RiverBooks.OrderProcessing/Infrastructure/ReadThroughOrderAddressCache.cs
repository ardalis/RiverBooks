using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal class ReadThroughOrderAddressCache : IOrderAddressCache
{
  private readonly RedisOrderAddressCache _redisCache;
  private readonly IMediator _mediator;
  private readonly ILogger<ReadThroughOrderAddressCache> _logger;

  public ReadThroughOrderAddressCache(RedisOrderAddressCache redisCache,
    IMediator mediator,
    ILogger<ReadThroughOrderAddressCache> logger)
  {
    _redisCache = redisCache;
    _mediator = mediator;
    _logger = logger;
  }

  public async Task<Result<OrderAddress>> GetByIdAsync(Guid id)
  {
    var result = await _redisCache.GetByIdAsync(id);
    if(result.IsSuccess) return result;

    if(result.Status == ResultStatus.NotFound)
    {
      // fetch data from source
      _logger.LogInformation("Address {id} not found; fetching from source.", id);
      var query = new Users.Contracts.UserAddressDetailsByIdQuery(id);

      var queryResult = await _mediator.Send(query);

      if (queryResult.IsSuccess)
      {
        var dto = queryResult.Value;
        var address = new Address(dto.Street1,
                                  dto.Street2,
                                  dto.City,
                                  dto.State,
                                  dto.PostalCode,
                                  dto.Country);
        var orderAddress = new OrderAddress(dto.AddressId, address);
        await StoreAsync(orderAddress);
        return orderAddress;
      }
    }
    return Result.NotFound();
  }

  public Task<Result> StoreAsync(OrderAddress orderAddress)
  {
    return _redisCache.StoreAsync(orderAddress);
  }
}
