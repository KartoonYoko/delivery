using DeliveryApp.Core.Application.Commands.AssignAnOrderToCourier;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignOrdersJob(IMediator mediator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var assignOrdersCommand = new AssignAnOrderToCourierCommand();
        await mediator.Send(assignOrdersCommand);
    }
}