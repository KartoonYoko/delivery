namespace DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;

public class GetUncompletedOrdersResponse
{
    public GetUncompletedOrdersResponse(List<OrderDto> orders)
    {
        Orders.AddRange(orders);
    }

    public List<OrderDto> Orders { get; set; } = new();
}

public class OrderDto
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Геопозиция (X,Y)
    /// </summary>
    public Location LocationDto { get; set; }
}

public class Location
{
    /// <summary>
    ///     Горизонталь
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     Вертикаль
    /// </summary>
    public int Y { get; set; }
}