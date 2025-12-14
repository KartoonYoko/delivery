using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Ports;

public interface IOrderRepository
{
    /// <summary>
    /// Добавить заказ
    /// </summary>
    /// <param name="order"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddOrderAsync(Order order, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить заказ
    /// </summary>
    /// <param name="order"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);

    /// <summary>
    /// Получить заказ по идентификатору
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить 1 любой заказ со статусом "Created"
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Order?> GetAnyCreatedOrderAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Получить все назначенные заказы (заказы со статусом "Assigned")
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<List<Order>> GetAllAssignedOrdersAsync(CancellationToken cancellationToken);
}