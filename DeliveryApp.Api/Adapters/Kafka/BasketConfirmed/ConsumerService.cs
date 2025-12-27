using Confluent.Kafka;
using DeliveryApp.Core.Application.Commands.CreateAnOrder;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Queues.Basket;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

public class ConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;
    private readonly ILogger<ConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ConsumerService(
        IOptions<Settings> settings,
        ILogger<ConsumerService> logger,
        IServiceScopeFactory scopeFactory
    )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        if (string.IsNullOrWhiteSpace(settings.Value.MessageBrokerHost))
            throw new ArgumentException(nameof(settings.Value.MessageBrokerHost));
        if (string.IsNullOrWhiteSpace(settings.Value.BasketConfirmedTopic))
            throw new ArgumentException(nameof(settings.Value.BasketConfirmedTopic));

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = settings.Value.MessageBrokerHost,
            GroupId = "BasketConsumerGroup",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _topic = settings.Value.BasketConfirmedTopic;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult.IsPartitionEOF) continue;

                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var integrationEvent = JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(
                    consumeResult.Message.Value
                );

                var createOrderCommand = CreateAnOrderCommand.Create(
                    Guid.NewGuid(),
                    integrationEvent.Address.Street,
                    integrationEvent.Volume
                );
                var result = await mediator.Send(createOrderCommand, cancellationToken);

                if (result.IsFailure)
                    _logger.LogError(
                        "Error creating order by  basket confirmation event. Basket id: {basketId}. Error: {error}",
                        integrationEvent.BasketId,
                        result.Error.Code
                    );

                try
                {
                    _consumer.StoreOffset(consumeResult);
                }
                catch (KafkaException e)
                {
                    _logger.LogError(e, "Store Offset error: {reason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogInformation(e, "Operation cancelled");
        }
    }
}