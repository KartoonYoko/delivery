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
    public async Task Can()
    {
        //Arrange


        //Act


        //Assert
    }
}