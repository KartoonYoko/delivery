namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

public sealed record OrderCreatedDomainEvent(Order Order) : DomainEvent;