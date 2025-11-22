using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel;

public record Location
{
    public int X { get; }

    public int Y { get; }

    private Location()
    {
    }

    private Location(int x, int y) : this()
    {
        X = x;
        Y = y;
    }

    public static Result<Location, Error> Create(int x, int y)
    {
        if (x < 1 || x > 10) return GeneralErrors.ValueIsRequired(nameof(x));
        if (y < 1 || y > 10) return GeneralErrors.ValueIsRequired(nameof(y));

        return new Location(x, y);
    }

    public static Location CreateRandom()
    {
        var random = new Random();

        var x = random.Next(1, 10);
        var y = random.Next(1, 10);

        return new Location(x, y);
    }

    public int DistanceTo(Location target)
    {
        return Math.Abs(target.X - X) + Math.Abs(target.Y - Y);
    }
}