using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public sealed class Courier : Aggregate<Guid>
{
    public const int DefaultBagSize = 10;

    public const string DefaultBagName = "Сумка";

    public string Name { get; private set; }

    public int Speed { get; private set; }

    public Location Location { get; private set; }

    public List<StoragePlace> StoragePlaces { get; private set; }

    private Courier()
    {
        Id = Guid.NewGuid();
        StoragePlaces = [];
    }

    private Courier(string name, int speed, Location location) : this()
    {
        Name = name;
        Speed = speed;
        Location = location;
    }

    public static Courier Create(string name, int speed, Location location)
    {
        var courier = new Courier(name, speed, location);

        courier.AddStoragePlace(StoragePlace.Create(DefaultBagName, DefaultBagSize).Value);

        return courier;
    }

    public void AddStoragePlace(StoragePlace storagePlace)
    {
        StoragePlaces.Add(storagePlace);
    }

    public bool CouldTakeOrder(Order order)
    {
        foreach (var storagePlace in StoragePlaces)
        {
            if (storagePlace.IsPossibleToPlaceOrder(order.Volume))
                return true;
        }

        return false;
    }

    public Result<object, Error> TakeOrder(Order order)
    {
        foreach (var storagePlace in StoragePlaces)
        {
            var result = storagePlace.PlaceOrder(order.Id, order.Volume);

            if (result.IsSuccess)
                return new object();
        }

        return Result.Failure<object, Error>(Errors.CouldNotTakeOrder());
    }

    public Result<object, Error> CompleteOrder(Order order)
    {
        foreach (var storagePlace in StoragePlaces)
        {
            if (storagePlace.OrderId == order.Id)
            {
                storagePlace.ExtractTheOrder();
                return new object();
            }
        }

        return Result.Failure<object, Error>(Errors.OrderNotFound());
    }

    public int EvaluateNumberOfStepsToDestination(Location destination)
    {
        var distance = Location.DistanceTo(destination);

        return distance / Speed;
    }

    public Result<object, Error> TakeStepTowardsDestination(Location destination)
    {
        var currentStamina = Speed;

        if (Location.X != destination.X && currentStamina > 0)
        {
            var isPositiveSign = (Location.X - destination.X) > 0;
            var distance = Math.Abs(Location.X - destination.X);
            var isDistanceLessOrEqualsThanSpeed = distance <= currentStamina;
            var lengthToGo = isDistanceLessOrEqualsThanSpeed
                ? currentStamina
                : distance;

            lengthToGo = isPositiveSign ? lengthToGo : lengthToGo * -1;

            var result = Location.Create(Location.X + lengthToGo, Location.Y);
            if (result.IsFailure)
                return result;

            Location = result.Value;

            currentStamina -= lengthToGo;
        }

        if (Location.Y != destination.Y && currentStamina > 0)
        {
            var isPositiveSign = (Location.Y - destination.Y) > 0;
            var distance = Math.Abs(Location.Y - destination.Y);
            var isDistanceLessOrEqualsThanSpeed = distance <= currentStamina;
            var lengthToGo = isDistanceLessOrEqualsThanSpeed
                ? currentStamina
                : distance;

            lengthToGo = isPositiveSign ? lengthToGo : lengthToGo * -1;

            var result = Location.Create(Location.X, Location.Y + lengthToGo);
            if (result.IsFailure)
                return result;

            Location = result.Value;
        }

        return new object();
    }

    public static class Errors
    {
        public static Error CouldNotTakeOrder()
        {
            return new Error(
                "courier.could.not.take.order",
                ""
            );
        }

        public static Error OrderNotFound()
        {
            return new Error(
                "order.not.found",
                ""
            );
        }
    }
}