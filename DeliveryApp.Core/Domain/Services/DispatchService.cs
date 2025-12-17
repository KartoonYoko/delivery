using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
    public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
    {
        if (order.Status != Status.Created) return GeneralErrors.ValueIsInvalid(nameof(order));
        if (couriers.Count == 0) return GeneralErrors.ValueIsInvalid(nameof(couriers));

        var couriersWhoCanTakeTheOrder = couriers
            .Where(x => x.CouldTakeOrder(order))
            .ToList();

        int currentStepsToDestination = int.MaxValue;
        Courier? currentCourier = null;
        foreach (var courier in couriersWhoCanTakeTheOrder)
        {
            var stepsToDestination = courier.EvaluateNumberOfStepsToDestination(order.Location);
            if (stepsToDestination < currentStepsToDestination)
            {
                currentStepsToDestination = stepsToDestination;
                currentCourier = courier;
            }
        }

        if (currentCourier == null) return Errors.CourierNotFound();

        currentCourier.TakeOrder(order);
        order.AssignToCourier(currentCourier.Id);

        return currentCourier;
    }

    public static class Errors
    {
        public static Error CourierNotFound()
        {
            return new Error(
                "courier.not.found",
                ""
            );
        }
    }
}