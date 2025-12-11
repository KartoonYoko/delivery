using DeliveryApp.Core.Domain.Model.CourierAggregate;

namespace DeliveryApp.Core.Ports;

public interface ICourierRepository
{
    /// <summary>
    /// Добавить курьера
    /// </summary>
    /// <param name="courier"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddCourierAsync(Courier courier, CancellationToken cancellationToken);
    
    /// <summary>
    /// Обновить курьера
    /// </summary>
    /// <param name="courier"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateCourierAsync(Courier courier, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получить курьера по идентификатору
    /// </summary>
    /// <param name="courierId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Courier?> GetCourierByIdAsync(Guid courierId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получить всех свободных курьеров (курьеры, у которых все места хранения свободны)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    IAsyncEnumerable<List<Courier>> GetAllFreeCouriersAsync(CancellationToken cancellationToken);
}