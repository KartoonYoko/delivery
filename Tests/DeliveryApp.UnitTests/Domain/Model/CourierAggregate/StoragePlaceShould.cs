using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate;

public class StoragePlaceShould
{
    [Fact]
    public void BeNotEqualWhenIdsAreNotEqual()
    {
        //Arrange
        var first = StoragePlace.Create("rucksack", 2).Value;
        var second = StoragePlace.Create("rucksack", 2).Value;

        //Act
        var result = first == second;

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("rucksack", 1)]
    [InlineData("backpack", 11, "c4adb8ef-d39e-4ea1-8c3c-5bb15ce2f6fa")]
    public void BeCorrectWhenParamsAreCorrectOnCreated(string name, int totalVolume, string? orderId = null)
    {
        //Arrange
        Guid? id = orderId is null ? null : Guid.Parse(orderId);

        //Act
        var result = StoragePlace.Create(name, totalVolume, id);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.TotalVolume.Should().Be(totalVolume);
        result.Value.OrderId.Should().Be(id);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("backpack", 0)]
    public void ReturnsErrorWhenParamsAreNotCorrectOnCreated(string name, int totalVolume)
    {
        //Arrange

        //Act
        var result = StoragePlace.Create(name, totalVolume);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void BeNotEqualAllPropertiesAreEqual()
    {
        //Arrange
        var first = StoragePlace.Create("backpack", 2).Value;
        var second = StoragePlace.Create("backpack", 2).Value;

        //Act
        var result = first == second;

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void BeEmptyWhenOrderIdIsEmpty()
    {
        //Arrange
        var first = StoragePlace.Create("backpack", 2).Value;

        //Act
        var result = first.IsEmpty();

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void BeNotEmptyWhenOrderIdIsNotEmpty()
    {
        //Arrange
        var first = StoragePlace.Create(
            "backpack",
            2,
            Guid.NewGuid()
        ).Value;

        //Act
        var result = first.IsEmpty();

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void BePossiblePlaceOrderWhenParamsAreCorrect()
    {
        //Arrange
        var sp = StoragePlace.Create("backpack", 2).Value;

        //Act
        var result = sp.PlaceOrder(Guid.NewGuid(), 2);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void BeNotPossiblePlaceOrderWhenOrderVolumeIsBigger()
    {
        //Arrange
        var orderVolume = 3;
        var spVolume = 2;

        var sp = StoragePlace.Create(
            "backpack",
            spVolume
        ).Value;

        //Act
        var result = sp.PlaceOrder(Guid.NewGuid(), orderVolume);

        //Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void BeNotPossiblePlaceOrderWhenPlaceAlreadyHasOrder()
    {
        //Arrange
        var sp = StoragePlace.Create(
            "backpack",
            4,
            Guid.NewGuid()
        ).Value;

        //Act
        var result = sp.PlaceOrder(Guid.NewGuid(), 2);

        //Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ShowPossibilityToPlaceOrderWhenOrderVolumeIsLessOrEqual()
    {
        //Arrange
        var orderVolume = 2;
        var spVolume = 2;

        var sp = StoragePlace.Create("backpack", spVolume).Value;

        //Act
        var result = sp.IsPossibleToPlaceOrder(orderVolume);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShowUnPossibilityToPlaceOrderWhenStoragePlaceAlreadyHasOrder()
    {
        //Arrange
        var sp = StoragePlace.Create(
            "backpack",
            2,
            Guid.NewGuid()
        ).Value;

        //Act
        var result = sp.IsPossibleToPlaceOrder(1);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ExtractOrder()
    {
        //Arrange
        var sp = StoragePlace.Create(
            "backpack",
            2,
            Guid.NewGuid()
        ).Value;

        //Act
        sp.ExtractTheOrder();

        //Assert
        sp.OrderId.Should().BeNull();
    }
}