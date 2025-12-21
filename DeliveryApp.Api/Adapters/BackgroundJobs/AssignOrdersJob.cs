using DeliveryApp.Core.Application.Commands.AssignAnOrderToCourier;
using DeliveryApp.Core.Domain.Services;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignOrdersJob(
    IMediator mediator,
    ILogger<AssignOrdersJob> logger
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var assignOrdersCommand = new AssignAnOrderToCourierCommand();
        var result = await mediator.Send(assignOrdersCommand);
        if (result.IsFailure)
            if (result.Error.Code == DispatchService.Errors.CourierNotFound().Code)
                logger.LogInformation(result.Error.Code);
            else
                logger.LogError(result.Error.Code);
    }
}