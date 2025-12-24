using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.Commands.CreateAnOrder;

public class CreateAnOrderHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IGeoClient geoClient
) : IRequestHandler<CreateAnOrderCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(CreateAnOrderCommand request, CancellationToken cancellationToken)
    {
        var geoResult = await geoClient.GetLocationByStreetAsync(request.Street, cancellationToken);
        if (geoResult.IsFailure)
            return UnitResult.Failure(geoResult.Error);
        
        var order = Order.Create(request.OrderId, Location.CreateRandom(), request.Volume);

        await orderRepository.AddOrderAsync(order, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}