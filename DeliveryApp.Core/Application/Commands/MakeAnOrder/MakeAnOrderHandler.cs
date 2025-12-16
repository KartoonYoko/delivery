using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.MakeAnOrder;

public class MakeAnOrderHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<MakeAnOrderCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(MakeAnOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.OrderId, Location.CreateRandom(), request.Volume);

        await orderRepository.AddOrderAsync(order, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}