using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries.GetAllCouriers;

public class GetAllCouriersHandler(ApplicationDbContext dbContext)
    : IRequestHandler<GetCouriersQuery, GetCouriersResponse>
{
    public async Task<GetCouriersResponse> Handle(GetCouriersQuery request, CancellationToken cancellationToken)
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