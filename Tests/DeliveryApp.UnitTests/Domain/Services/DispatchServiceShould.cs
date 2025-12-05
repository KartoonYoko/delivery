using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceShould
{
    [Fact]
    public void CorrectlyDispatch()
    {
        // Arrange
        var couriersLocation = Location.Create(1, 1).Value;
        var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5).Value, 5);
        var dispatchService = new DispatchService();

        var courier = Courier.Create("1", 3, couriersLocation).Value;
        var fastestCourier = Courier.Create("1", 5, couriersLocation).Value;

        // Act
        var result = dispatchService.Dispatch(order, [courier, fastestCourier]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(fastestCourier);
    }

    [Fact]
    public void ReturnsErrorWhenOrderHasInvalidStatus()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5).Value, 5);
        order.AssignToCourier(Guid.NewGuid());
        var dispatchService = new DispatchService();
        var courier = Courier.Create("1", 3, Location.CreateRandom()).Value;

        // Act
        var result = dispatchService.Dispatch(order, [courier]);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReturnsErrorWhenCouriersCanNotTakeOrder()
    {
        // Arrange
        var orderVolumeThatIsBiggerThanCourierVolume = 100;
        var order = Order.Create(
            Guid.NewGuid(),
            Location.CreateRandom(),
            orderVolumeThatIsBiggerThanCourierVolume
        );
        var dispatchService = new DispatchService();
        var courier = Courier.Create("1", 3, Location.CreateRandom()).Value;

        // Act
        var result = dispatchService.Dispatch(order, [courier]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(DispatchService.Errors.CourierNotFound().Code);
    }
}