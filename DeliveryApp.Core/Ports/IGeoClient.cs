using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    Task<Result<Location, Error>> GetLocationByStreetAsync(string street, CancellationToken cancellationToken);
}