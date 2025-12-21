using System.Runtime.CompilerServices;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    public async Task AddOrderAsync(Order order, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(order, cancellationToken);
    }

    public Task UpdateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Update(order);

        return Task.CompletedTask;
    }

    public Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
    }

    public Task<Order?> GetAnyCreatedOrderAsync(CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .Where(x => x.Status.Name == Status.Created.Name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async IAsyncEnumerable<List<Order>> GetAllAssignedOrdersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        const int take = 1000;
        var skip = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await dbContext.Orders
                .Where(x => x.Status.Name == Status.Assigned.Name)
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);

            if (batch.Count == 0)
                break;

            yield return batch;

            skip += take;
        }
    }
}