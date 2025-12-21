using DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries.GetAllUncompletedOrders;

public class GetAllUncompletedOrdersHandler(ApplicationDbContext dbContext)
    : IRequestHandler<GetUncompletedOrdersQuery, GetUncompletedOrdersResponse>
{
    public async Task<GetUncompletedOrdersResponse> Handle(
        GetUncompletedOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        var items = await dbContext.Orders
            .Where(x => x.Status.Name != Status.Completed.Name)
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