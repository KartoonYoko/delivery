using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate<Guid>
{
    public Location Location { get; private set; }

    public int Volume { get; private set; }

    public Status Status { get; private set; } = Status.Created;

    public Guid? CourierId { get; private set; }

    private Order()
    {
    }

    private Order(Guid id, Location location, int volume)
    {
        Id = id;
        Location = location;
        Volume = volume;
    }

    public static Order Create(Guid id, Location location, int volume)
    {
        var order = new Order(id, location, volume);

        return order;
    }

    public void AssignToCourier(Guid courierId)
    {
        CourierId = courierId;
        Status = Status.Assigned;
    }

    public UnitResult<Error> Complete()
    {
        if (Status != Status.Assigned)
            return UnitResult.Failure<Error>(Errors.OrderCouldNotBeComplete());

        Status = Status.Completed;

        return UnitResult.Success<Error>();
    }

    public static class Errors
    {
        public static Error OrderCouldNotBeComplete()
        {
            return new Error(
                "order.could.not.be.completed",
                ""
            );
        }
    }
}