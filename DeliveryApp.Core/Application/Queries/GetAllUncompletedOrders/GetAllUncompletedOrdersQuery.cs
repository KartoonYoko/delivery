using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;

public class GetAllUncompletedOrdersQuery(IGetAllUncompletedOrdersQuery query)
{
    public Task<GetUncompletedOrdersResponse> HandleAsync(CancellationToken cancellationToken = default)
        => query.Handle(cancellationToken);
}