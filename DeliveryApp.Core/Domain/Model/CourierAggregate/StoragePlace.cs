using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public sealed class StoragePlace : Entity<Guid>
{
    private const int MinimalTotalVolume = 1;

    public string Name { get; private set; }

    public int TotalVolume { get; private set; }

    public Guid? OrderId { get; private set; }

    private StoragePlace()
    {
        Id = Guid.NewGuid();
    }

    public static Result<StoragePlace, Error> Create(string name, int totalVolume, Guid? orderId = null)
    {
        if (totalVolume < MinimalTotalVolume) return GeneralErrors.ValueIsInvalid(nameof(totalVolume));
        if (string.IsNullOrEmpty(name)) return GeneralErrors.ValueIsInvalid(nameof(name));

        return new StoragePlace
        {
            Name = name,
            TotalVolume = totalVolume,
            OrderId = orderId,
        };
    }

    public bool IsEmpty()
    {
        return OrderId is null;
    }

    public bool IsPossibleToPlaceOrder(int orderVolume)
    {
        var result = CheckPossibilityToPlaceOrder(orderVolume);

        return result.IsSuccess;
    }

    public Result<object, Error> PlaceOrder(Guid orderId, int orderVolume)
    {
        var result = CheckPossibilityToPlaceOrder(orderVolume);

        if (result.IsFailure)
            return Result.Failure<object, Error>(Errors.OrderCouldNotBePlaced(result.Error));

        OrderId = orderId;

        return new object();
    }

    public void ExtractTheOrder()
    {
        OrderId = null;
    }

    private Result<object, List<Error>> CheckPossibilityToPlaceOrder(int orderVolume)
    {
        // Место хранения должно уметь проверять, можно ли в него поместить заказ и возвращать да или нет.
        // Если в месте хранения уже есть другой заказ
        // или объем заказа превышает объем места хранения, то поместить новый нельзя.
        List<Error> errors = [];

        if (OrderId is not null)
            errors.Add(Errors.OrderAlreadyExists());
        if (orderVolume > TotalVolume)
            errors.Add(Errors.OrderVolumeIsMoreThanPlaceVolume());

        return errors.Count == 0
            ? new object()
            : Result.Failure<object, List<Error>>(errors);
    }

    public static class Errors
    {
        public static Error OrderCouldNotBePlaced(List<Error> errors)
        {
            return new Error(
                "order.could.not.be.placed",
                "",
                errors
            );
        }

        public static Error OrderAlreadyExists()
        {
            return new Error(
                "order.already.exists",
                ""
            );
        }

        public static Error OrderVolumeIsMoreThanPlaceVolume()
        {
            return new Error(
                "order.volume.more.than.place.volume",
                ""
            );
        }
    }
}