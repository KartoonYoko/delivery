using System;
using System.Linq;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate;

public class CourierShould
{
    [Fact]
    public void ContainsBag()
    {
        //Arrange

        //Act
        var courier = Courier.Create("Some courier", 2, Location.CreateRandom()).Value;
        var bag = courier.StoragePlaces.FirstOrDefault();

        //Assert
        bag.Should().NotBeNull();
        bag.Name.Should().Be(Courier.DefaultBagName);
        bag.TotalVolume.Should().Be(Courier.DefaultBagSize);
    }

    [Fact]
    public void AddStoragePlace()
    {
        //Arrange
        var courier = Courier.Create("Some courier", 2, Location.CreateRandom()).Value;
        var spName = "Место хранения";
        var spVolume = 5;

        //Act
        var result = courier.AddStoragePlace(spName, spVolume);
        var sp = courier.StoragePlaces[1];

        //Assert
        result.IsSuccess.Should().BeTrue();
        sp.Should().NotBeNull();
        sp.Name.Should().Be(spName);
        sp.TotalVolume.Should().Be(spVolume);
    }

    [Fact]
    public void TakeOrderWhenPossible()
    {
        //Arrange
        var courier = Courier.Create("Some courier", 2, Location.CreateRandom()).Value;
        var orderVolumeThatLessThanDefaultBagSize = Courier.DefaultBagSize - 1;
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), orderVolumeThatLessThanDefaultBagSize);

        //Act
        var result = courier.TakeOrder(order);

        //Assert
        result.IsSuccess.Should().BeTrue();
        courier.StoragePlaces.FirstOrDefault()?.OrderId.Should().Be(order.Id);
    }

    [Fact]
    public void NotTakeOrderWhenNotPossible()
    {
        //Arrange
        var courier = Courier.Create("Some courier", 2, Location.CreateRandom()).Value;
        var orderVolumeThatMoreThanDefaultBagSize = Courier.DefaultBagSize + 1;
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), orderVolumeThatMoreThanDefaultBagSize);

        //Act
        var result = courier.TakeOrder(order);

        //Assert
        result.IsSuccess.Should().BeFalse();
        courier.StoragePlaces.FirstOrDefault()?.OrderId.Should().BeNull();
    }

    [Fact]
    public void CompleteOrderWhenHeContainsTheOrder()
    {
        //Arrange
        var courier = Courier.Create("Some courier", 2, Location.CreateRandom()).Value;
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), Courier.DefaultBagSize - 1);
        var resultOfTaking = courier.TakeOrder(order);
        resultOfTaking.IsSuccess.Should().BeTrue();

        //Act
        var result = courier.CompleteOrder(order);

        //Assert
        result.IsSuccess.Should().BeTrue();
        courier.StoragePlaces.FirstOrDefault()?.OrderId.Should().BeNull();
    }

    [Fact]
    public void NotCompleteOrderWhenHeDoesNotContainsTheOrder()
    {
        //Arrange
        var courier = Courier.Create("Some courier", 2, Location.CreateRandom()).Value;
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), Courier.DefaultBagSize - 1);

        //Act
        var result = courier.CompleteOrder(order);

        //Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 1, 5, 5, 4)]
    [InlineData(5, 5, 1, 1, 4)]
    public void EvaluateNumberOfStepsToDestinationCorrectly(
        int locationX,
        int locationY,
        int destinationX,
        int destinationY,
        int correctResult
    )
    {
        //Arrange
        var courier = Courier.Create(
            "Some courier",
            2,
            Location.Create(locationX, locationY).Value
        ).Value;

        //Act
        var result = courier.EvaluateNumberOfStepsToDestination(Location.Create(destinationX, destinationY).Value);

        //Assert
        result.Should().Be(correctResult);
    }

    [Theory]
    [InlineData(1, 1, 3, 3, 3, 1, 2)]
    [InlineData(3, 3, 1, 1, 1, 3, 2)]
    [InlineData(1, 1, 3, 3, 3, 2, 3)]
    [InlineData(1, 1, 3, 3, 3, 3, 4)]
    public void TakeStepTowardsDestinationCorrectly(
        int startX,
        int startY,
        int destinationX,
        int destinationY,
        int correctX,
        int correctY,
        int speed
    )
    {
        //Arrange
        var courier = Courier.Create(
            "Some courier",
            speed,
            Location.Create(startX, startY).Value
        ).Value;

        //Act
        var result = courier.TakeStepTowardsDestination(Location.Create(destinationX, destinationY).Value);

        //Assert
        result.IsSuccess.Should().BeTrue();
        courier.Location.X.Should().Be(correctX);
        courier.Location.Y.Should().Be(correctY);
    }
}