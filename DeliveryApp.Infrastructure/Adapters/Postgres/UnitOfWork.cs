using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork(ApplicationDbContext dbContext, IMediator mediator) : IUnitOfWork
{
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync(cancellationToken);
        return true;
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        // Получили агрегаты в которых есть доменные события
        var domainEntities = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Any());

        // Переложили в отдельную переменную
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        // Очистили Domain Event в самих агрегатах (поскольку далее они будут отправлены и больше не нужны)
        domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

        // Отправили в MediatR
        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent, cancellationToken);
    }
}