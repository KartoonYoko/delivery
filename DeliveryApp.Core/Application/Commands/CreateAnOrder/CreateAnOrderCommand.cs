namespace DeliveryApp.Core.Application.Commands.CreateAnOrder;

public class CreateAnOrderCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    ///     Идентификатор заказа
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    ///     Улица
    /// </summary>
    /// <remarks>Корзина содержала полный Address, но для упрощения мы будем использовать только Street из Address</remarks>
    public string Street { get; private set; }

    /// <summary>
    ///     Объем
    /// </summary>
    public int Volume { get; private set; }

    public static CreateAnOrderCommand Create(Guid orderId, string street, int volume)
    {
        var command = new CreateAnOrderCommand
        {
            OrderId = orderId,
            Street = street,
            Volume = volume,
        };

        return command;
    }
}