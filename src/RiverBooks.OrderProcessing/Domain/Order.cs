using System.ComponentModel.DataAnnotations.Schema;
using RiverBooks.SharedKernel;
using Ardalis.GuardClauses;

namespace RiverBooks.OrderProcessing.Domain;

internal class Order : IHaveDomainEvents
{
  private Order() { }

  public Guid Id { get; private set; } = Guid.NewGuid();
  public Guid UserId { get; private set; }
  public Address ShippingAddress { get; private set; } = default!;
  public Address BillingAddress { get; private set; } = default!;
  private readonly List<OrderItem> _orderItems = new();
  public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

  public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;

  private void AddOrderItem(OrderItem item) => _orderItems.Add(item);

  private List<DomainEventBase> _domainEvents = new();
  [NotMapped]
  public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
  void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();

  public class Factory
  {
    public static Order Create(Guid userId,
                               Address shippingAddress,
                               Address billingAddress,
                               IEnumerable<OrderItem> orderItems)
    {
      if(!orderItems.Any())
      {
        throw new ArgumentException("Must have some order items", nameof(orderItems));
      }
      var order = new Order();
      order.UserId = userId;
      order.ShippingAddress = shippingAddress;
      order.BillingAddress = billingAddress;
      foreach (var item in orderItems)
      {
        order.AddOrderItem(item);
      }
      // uncomment this to make archunit test fail
      //var db = new OrderProcessingDbContext(
      //  new Microsoft.EntityFrameworkCore.DbContextOptions<OrderProcessingDbContext>());

      var orderCreatedEvent = new OrderCreatedEvent(order);
      order.RegisterDomainEvent(orderCreatedEvent);
      
      return order;
    }
  }
}
