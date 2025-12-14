using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("basket")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private ApplicationDbContext _context = null!;

    private readonly CancellationToken _token = CancellationToken.None;

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync(_token);

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); }
            )
            .UseSnakeCaseNamingConvention()
            .Options;

        _context = new ApplicationDbContext(contextOptions);
        await _context.Database.MigrateAsync(_token);
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanAddOrder()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value, 10);
        var repository = new OrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        //Act
        await repository.AddOrderAsync(order, _token);
        await unitOfWork.SaveChangesAsync(_token);

        //Assert
        var check = _context.Orders
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == order.Id);
        check.Should().NotBeNull();
        check.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task CanUpdateOrder()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value, 10);
        var courier = Courier.Create("courier", 2, Location.Create(1, 1).Value).Value;
        var repository = new OrderRepository(_context);
        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await courierRepository.AddCourierAsync(courier, _token);
        await repository.AddOrderAsync(order, _token);
        await unitOfWork.SaveChangesAsync(_token);

        //Act
        order.AssignToCourier(courier.Id);
        order.Complete();
        await repository.UpdateOrderAsync(order, _token);
        await unitOfWork.SaveChangesAsync(_token);

        //Assert
        var check = _context.Orders
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == order.Id);
        check.Should().NotBeNull();
        check.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task CanGetOrderById()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value, 10);
        var repository = new OrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await repository.AddOrderAsync(order, _token);
        await unitOfWork.SaveChangesAsync(_token);

        //Act
        var check = _context.Orders
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == order.Id);

        //Assert
        check.Should().NotBeNull();
        check.Should().BeEquivalentTo(order);
    }
    
    [Fact]
    public async Task CanGetAllAssignedOrders()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value, 10);
        var courier = Courier.Create("courier", 2, Location.Create(1, 1).Value).Value;
        var repository = new OrderRepository(_context);
        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await courierRepository.AddCourierAsync(courier, _token);
        await repository.AddOrderAsync(order, _token);
        await unitOfWork.SaveChangesAsync(_token);

        //Act
        var enumerable = repository.GetAllAssignedOrdersAsync(_token);
        List<Order> list = [];
        await foreach (var orders in enumerable)
        {
            list.AddRange(orders);
        }

        //Assert
        list.Count.Should().Be(1);
        var check = list.FirstOrDefault();
        check.Should().NotBeNull();
        check.Should().BeEquivalentTo(order);
    }
}