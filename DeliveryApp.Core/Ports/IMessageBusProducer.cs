using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports;

public interface IMessageBusProducer
{
    Task Publish(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken);
    
    Task Publish(OrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken);
}