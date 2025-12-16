using DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries.GetAllUncompletedOrders;

public class GetAllUncompletedOrdersQuery(ApplicationDbContext dbContext) : IGetAllUncompletedOrdersQuery
{
    public async Task<GetUncompletedOrdersResponse> Handle(CancellationToken cancellationToken)
    {
        var items = await dbContext.Orders
            .Where(x => x.Status != Status.Created)
            .Where(x => x.Status != Status.Completed)
            .Select(x => new OrderDto
            {
                Id = x.Id,
                LocationDto = new Location
                {
                    X = x.Location.X,
                    Y = x.Location.Y,
                }
            })
            .ToListAsync(cancellationToken);

        return new GetUncompletedOrdersResponse(items);
    }
}