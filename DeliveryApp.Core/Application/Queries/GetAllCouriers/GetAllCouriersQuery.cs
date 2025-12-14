using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

public class GetAllCouriersQuery(IGetAllCouriersQuery query)
{
    public Task<GetCouriersResponse> HandleAsync(CancellationToken cancellationToken = default)
        => query.Handle(cancellationToken);
}