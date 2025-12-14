using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryShould : IAsyncLifetime
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
    public async Task CanAddCourier()
    {
        //Arrange
        var location = Location.Create(5, 5).Value;
        var courier = Courier.Create("courier", 10, location).Value;

        //Act
        var repository = new CourierRepository(_context);
        await repository.AddCourierAsync(courier, _token);
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync(_token);

        //Assert
        var check = await repository.GetCourierByIdAsync(courier.Id, _token);
        check.Should().NotBeNull();
        check.Should().BeEquivalentTo(courier);
    }

    [Fact]
    public async Task CanUpdateCourier()
    {
        //Arrange
        const string storagePlaceName = "new_storage_place";
        const int storagePlaceVolume = 15;
        var location = Location.Create(5, 5).Value;
        var courier = Courier.Create("courier", 2, location).Value;

        var repository = new CourierRepository(_context);
        await repository.AddCourierAsync(courier, _token);
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync(_token);

        //Act
        var addResult = courier.AddStoragePlace(storagePlaceName, storagePlaceVolume);
        courier.TakeStepTowardsDestination(Location.Create(1, 1).Value);
        await repository.UpdateCourierAsync(courier, _token);
        await unitOfWork.SaveChangesAsync(_token);

        //Assert
        addResult.IsSuccess.Should().BeTrue();
        var check = await repository.GetCourierByIdAsync(courier.Id, _token);
        check.Should().NotBeNull();
        check.StoragePlaces.Count.Should().Be(2);
        var addedStoragePlace = check.StoragePlaces.FirstOrDefault(x => x.Name == storagePlaceName);
        addedStoragePlace.Should().NotBeNull();
        addedStoragePlace.Name.Should().Be(storagePlaceName);
        addedStoragePlace.TotalVolume.Should().Be(storagePlaceVolume);
    }

    [Fact]
    public async Task CanGetCourierById()
    {
        //Arrange
        var location = Location.Create(5, 5).Value;
        var courier = Courier.Create("courier", 10, location).Value;

        var repository = new CourierRepository(_context);
        await repository.AddCourierAsync(courier, _token);
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync(_token);

        //Act
        var check = _context.Couriers
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == courier.Id);

        //Assert
        check.Should().NotBeNull();
        check.Should().BeEquivalentTo(courier);
    }
}