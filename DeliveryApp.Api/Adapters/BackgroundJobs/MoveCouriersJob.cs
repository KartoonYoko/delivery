using DeliveryApp.Core.Application.Commands.MoveCouriers;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class MoveCouriersJob(IMediator mediator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var moveCourierToOrderCommand = new MoveCouriersCommand();
        await mediator.Send(moveCourierToOrderCommand);
    }
}