﻿namespace RiverBooks.Users.UseCases;

internal record OrderSummary
{
  public Guid UserId { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public DateTimeOffset? DateShipped { get; set; }
  public decimal Total { get; set; }
}