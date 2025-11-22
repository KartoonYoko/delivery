using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public class LocationShould
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    public void BeCorrectWhenParamsAreCorrectOnCreated(int x, int y)
    {
        //Arrange

        //Act
        var result = Location.Create(x, y);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(x);
        result.Value.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(11, 10)]
    [InlineData(10, 11)]
    public void ReturnsErrorWhenParamsAreNotCorrectOnCreated(int x, int y)
    {
        //Arrange

        //Act
        var result = Location.Create(x, y);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }


    [Fact]
    public void BeEqualWhenAllPropertiesIsEqual()
    {
        //Arrange
        var first = Location.Create(1, 2).Value;
        var second = Location.Create(1, 2).Value;

        //Act
        var result = first == second;

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void BeNotEqualWhenAnyPropertiesAreNotEqual()
    {
        //Arrange
        var first = Location.Create(1, 2).Value;
        var second = Location.Create(2, 1).Value;

        //Act
        var result = first == second;

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(2, 6, 4, 9, 5)]
    [InlineData(4, 9, 2, 6, 5)]
    public void CalculatesCorrectDistance(int startX, int startY, int endX, int endY, int rightDistance)
    {
        //Arrange
        var start = Location.Create(startX, startY).Value;
        var end = Location.Create(endX, endY).Value;

        //Act
        var distance = start.DistanceTo(end);

        //Assert
        distance.Should().Be(rightDistance);
    }
}