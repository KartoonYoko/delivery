using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Status : ValueObject
{
    public static Status Created => new("Created");
    public static Status Assigned => new("Assigned");
    public static Status Completed => new("Completed");

    private Status()
    {
    }

    private Status(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}