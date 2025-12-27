using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public class OrderCreatedDomainEventHandler(IMessageBusProducer messageBusProducer)
    : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await messageBusProducer.Publish(notification, cancellationToken);
    }
}