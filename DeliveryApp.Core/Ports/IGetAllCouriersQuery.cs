using DeliveryApp.Core.Application.Queries.GetAllCouriers;

namespace DeliveryApp.Core.Ports;

public interface IGetAllCouriersQuery
{
    Task<GetCouriersResponse> Handle(CancellationToken cancellationToken);
}