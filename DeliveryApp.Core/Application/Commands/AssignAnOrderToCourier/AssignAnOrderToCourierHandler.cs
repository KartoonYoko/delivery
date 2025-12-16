using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.Commands.AssignAnOrderToCourier;

public class AssignAnOrderToCourierHandler(
    IOrderRepository orderRepository,
    ICourierRepository courierRepository,
    IDispatchService dispatchService,
    IUnitOfWork unitOfWork
)
    : IRequestHandler<AssignAnOrderToCourierCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(
        AssignAnOrderToCourierCommand request,
        CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var order = await orderRepository.GetAnyCreatedOrderAsync(cancellationToken);
            if (order is null)
                break;

            var dispatchResult = await DispatchCourierAsync(order, cancellationToken);
            if (dispatchResult.IsFailure)
                return UnitResult.Failure(dispatchResult.Error);

            await courierRepository.UpdateCourierAsync(dispatchResult.Value, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return UnitResult.Success<Error>();
    }

    async Task<Result<Courier, Error>> DispatchCourierAsync(Order order, CancellationToken cancellationToken)
    {
        Courier? courierToDispatch = null;

        IAsyncEnumerable<List<Courier>> enumerable = courierRepository.GetAllFreeCouriersAsync(cancellationToken);
        await foreach (var freeCouriers in enumerable)
        {
            var dispatchResult = dispatchService.Dispatch(order, freeCouriers);

            if (dispatchResult.IsFailure)
            {
                if (dispatchResult.Error == DispatchService.Errors.CourierNotFound())
                    continue;

                return Result.Failure<Courier, Error>(dispatchResult.Error);
            }

            courierToDispatch = dispatchResult.Value;
            break;
        }

        return courierToDispatch is null
            ? Result.Failure<Courier, Error>(Errors.CourierNotFound())
            : Result.Success<Courier, Error>(courierToDispatch);
    }

    public static class Errors
    {
        public static Error CourierNotFound() => new Error("courier.not.found", "");
    }
}