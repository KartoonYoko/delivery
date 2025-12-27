using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public class OrderCompletedDomainEventHandler(IMessageBusProducer messageBusProducer)
    : INotificationHandler<OrderCompletedDomainEvent>
{
    public async Task Handle(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await messageBusProducer.Publish(notification, cancellationToken);
    }
}