using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.Commands.CreateAnOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application;

public class MakeAnOrderCommandShould
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();


    [Fact]
    public async Task CreateOrder()
    {
        //Arrange
        var command = CreateAnOrderCommand.Create(Guid.NewGuid(), "some street", 10);
        var handler = new CreateAnOrderHandler(_orderRepository, _unitOfWork);
        _orderRepository.AddOrderAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        _unitOfWork.SaveChangesAsync()
            .Returns(Task.FromResult(true));

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}