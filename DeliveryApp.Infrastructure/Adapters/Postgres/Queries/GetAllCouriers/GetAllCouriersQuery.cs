using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries.GetAllCouriers;

public class GetAllCouriersQuery(ApplicationDbContext dbContext) : IGetAllCouriersQuery
{
    public async Task<GetCouriersResponse> Handle(CancellationToken cancellationToken)
    {
        var items = await dbContext.Couriers
            .Select(x => new Courier
            {
                Id = x.Id,
                Name = x.Name,
                Location = new Location
                {
                    X = x.Location.X,
                    Y = x.Location.Y,
                }
            })
            .ToListAsync(cancellationToken);
        
        return new GetCouriersResponse(items);
    }
}