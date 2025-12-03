using System;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate;

public class OrderShould
{
    [Fact]
    public void CreatesCorrectly()
    {
        //Arrange
        var guid = Guid.NewGuid();
        var location = Location.CreateRandom();
        var volume = 10;

        //Act
        var order = Order.Create(guid, location, volume);

        //Assert
        order.Location.Should().Be(location);
        order.Volume.Should().Be(volume);
        order.Id.Should().Be(guid);
    }

    [Fact]
    public void AssignToCourier()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), 10);
        var courierId = Guid.NewGuid();

        //Act
        order.AssignToCourier(courierId);

        //Assert
        order.CourierId.Should().Be(courierId);
    }

    [Fact]
    public void CompletesWhenAlreadyAssignedToCourier()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), 10);
        var courierId = Guid.NewGuid();
        order.AssignToCourier(courierId);

        //Act
        var result = order.Complete();

        //Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(Status.Completed);
    }

    [Fact]
    public void NotCompletesWhenHasNotAssignedCourier()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), 10);

        //Act
        var result = order.Complete();

        //Assert
        result.IsFailure.Should().BeTrue();
        order.Status.Should().NotBe(Status.Completed);
    }
}