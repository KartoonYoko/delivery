using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.MoveCouriers;

public class MoveCouriersHandler(
    IUnitOfWork unitOfWork,
    ICourierRepository courierRepository,
    IOrderRepository orderRepository
)
    : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
    {
        IAsyncEnumerable<List<Order>> enumerable = orderRepository.GetAllAssignedOrdersAsync(cancellationToken);

        await foreach (var orders in enumerable)
        {
            foreach (var order in orders)
            {
                if (order.CourierId is null)
                    continue;

                var courier = await courierRepository.GetCourierByIdAsync(order.CourierId.Value, cancellationToken);

                if (courier is null)
                    continue;

                if (courier.Location == order.Location)
                {
                    var completeOrderResult = order.Complete();
                    if (completeOrderResult.IsFailure)
                        return UnitResult.Failure(completeOrderResult.Error);

                    var completeOrderByCourierResult = courier.CompleteOrder(order);
                    if (completeOrderByCourierResult.IsFailure)
                        return UnitResult.Failure(completeOrderByCourierResult.Error);
                }
                else
                {
                    courier.TakeStepTowardsDestination(order.Location);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return UnitResult.Success<Error>();
    }
}