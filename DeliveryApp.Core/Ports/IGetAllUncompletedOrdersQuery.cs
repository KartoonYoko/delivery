using DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;

namespace DeliveryApp.Core.Ports;

public interface IGetAllUncompletedOrdersQuery
{
    Task<GetUncompletedOrdersResponse> Handle(CancellationToken cancellationToken);
}