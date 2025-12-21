using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.Commands.CreateAnOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application;

public class CreateAnOrderCommandShould
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGeoClient _geoClient = Substitute.For<IGeoClient>();


    [Fact]
    public async Task CreateOrderSuccessfully()
    {
        //Arrange
        var command = CreateAnOrderCommand.Create(Guid.NewGuid(), "Тестировочная", 10);
        var handler = new CreateAnOrderHandler(_orderRepository, _unitOfWork, _geoClient);
        _orderRepository.AddOrderAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));
        _geoClient.GetLocationByStreetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Location.Create(1, 1)));

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}