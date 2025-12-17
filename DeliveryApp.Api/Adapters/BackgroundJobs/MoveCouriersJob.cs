using DeliveryApp.Core.Application.Commands.MoveCouriers;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class MoveCouriersJob(
    IMediator mediator,
    ILogger<MoveCouriersJob> logger
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var moveCourierToOrderCommand = new MoveCouriersCommand();
        var result = await mediator.Send(moveCourierToOrderCommand);
        if (result.IsFailure)
            logger.LogError(result.Error.Code);
    }
}