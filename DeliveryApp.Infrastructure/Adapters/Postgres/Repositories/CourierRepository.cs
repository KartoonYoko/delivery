using System.Runtime.CompilerServices;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class CourierRepository(ApplicationDbContext dbContext) : ICourierRepository
{
    public async Task AddCourierAsync(Courier courier, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(courier, cancellationToken);
    }

    public Task UpdateCourierAsync(Courier courier, CancellationToken cancellationToken)
    {
        dbContext.Update(courier);

        return Task.CompletedTask;
    }

    public Task<Courier?> GetCourierByIdAsync(Guid courierId, CancellationToken cancellationToken)
    {
        return dbContext.Couriers
            .Include(x => x.StoragePlaces)
            .FirstOrDefaultAsync(x => x.Id == courierId, cancellationToken);
    }

    public async IAsyncEnumerable<List<Courier>> GetAllFreeCouriersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        const int take = 1000;
        var skip = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await dbContext.Couriers
                .Include(x => x.StoragePlaces)
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