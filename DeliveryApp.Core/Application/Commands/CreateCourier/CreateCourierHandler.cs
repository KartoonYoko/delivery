using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Core.Application.Commands.CreateCourier;

public class CreateCourierHandler(
    ICourierRepository repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateCourierCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(CreateCourierCommand request, CancellationToken cancellationToken)
    {
        var createCourierResult = Courier.Create(request.Name, request.Speed, Location.CreateRandom());
        if (createCourierResult.IsFailure)
            return UnitResult.Failure(createCourierResult.Error);

        await repository.AddCourierAsync(createCourierResult.Value, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}