using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Queues.Order;

namespace DeliveryApp.Infrastructure.Adapters.Kafka;

public sealed class Producer : IMessageBusProducer
{
    private readonly ProducerConfig _config;
    private readonly string _topicName;
    private readonly ILogger<Producer> _logger;

    public Producer(IOptions<Settings> options, ILogger<Producer> logger)
    {
        if (string.IsNullOrWhiteSpace(options.Value.MessageBrokerHost))
            throw new ArgumentException(nameof(options.Value.MessageBrokerHost));
        if (string.IsNullOrWhiteSpace(options.Value.BasketConfirmedTopic))
            throw new ArgumentException(nameof(options.Value.BasketConfirmedTopic));

        _config = new ProducerConfig
        {
            BootstrapServers = options.Value.MessageBrokerHost
        };
        _topicName = options.Value.OrderStatusChangedTopic;
        _logger = logger;
    }

    public async Task Publish(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderCreatedIntegrationEvent
        {
            OrderId = domainEvent.Order.Id.ToString(),
        };
        // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = domainEvent.EventId.ToString(),
            Value = JsonConvert.SerializeObject(integrationEvent),
        };

        try
        {
            // Отправляем сообщение в Kafka
            using var producer = new ProducerBuilder<string, string>(_config).Build();
            var dr = await producer.ProduceAsync(_topicName, message, cancellationToken);
            _logger.LogInformation(
                "Delivered '{DrValue}' to '{DrTopicPartitionOffset}'", dr.Value, dr.TopicPartitionOffset
            );
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError("Delivery failed: {reason}", e.Message);
        }
    }

    public async Task Publish(OrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderCompletedIntegrationEvent
        {
            OrderId = domainEvent.Order.Id.ToString(),
        };
        // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = domainEvent.EventId.ToString(),
            Value = JsonConvert.SerializeObject(integrationEvent),
        };

        try
        {
            // Отправляем сообщение в Kafka
            using var producer = new ProducerBuilder<string, string>(_config).Build();
            var dr = await producer.ProduceAsync(_topicName, message, cancellationToken);
            _logger.LogInformation(
                "Delivered '{DrValue}' to '{DrTopicPartitionOffset}'", dr.Value, dr.TopicPartitionOffset
            );
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError("Delivery failed: {reason}", e.Message);
        }
    }
}