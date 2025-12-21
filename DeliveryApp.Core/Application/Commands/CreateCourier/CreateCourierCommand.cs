namespace DeliveryApp.Core.Application.Commands.CreateCourier;

public class CreateCourierCommand : IRequest<UnitResult<Error>>
{
    public string Name { get; private set; }

    public int Speed { get; private set; }

    public static CreateCourierCommand Create(string name, int speed)
    {
        return new CreateCourierCommand
        {
            Name = name,
            Speed = speed,
        };
    }
}