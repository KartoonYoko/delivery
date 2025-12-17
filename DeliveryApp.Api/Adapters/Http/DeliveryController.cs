using DeliveryApp.Core.Application.Commands.CreateAnOrder;
using DeliveryApp.Core.Application.Commands.CreateCourier;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController(IMediator mediator) : DefaultApiController
{
    public override async Task<IActionResult> CreateCourier(NewCourier newCourier)
    {
        var command = CreateCourierCommand.Create(newCourier.Name, newCourier.Speed);
        var result = await mediator.Send(command);
        if (result.IsFailure)
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        
        return Ok();
    }

    public override async Task<IActionResult> CreateOrder()
    {
        var orderId = Guid.NewGuid();
        var street = "Несуществующая";
        var volume = 10;

        var command = CreateAnOrderCommand.Create(orderId, street, volume);
        var result = await mediator.Send(command);
        if (result.IsFailure)
            return Problem(statusCode: StatusCodes.Status500InternalServerError);

        return Ok();
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var query = new GetCouriersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    public override async Task<IActionResult> GetOrders()
    {
        var query = new GetUncompletedOrdersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}