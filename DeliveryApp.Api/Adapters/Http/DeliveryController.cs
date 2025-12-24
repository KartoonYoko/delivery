using DeliveryApp.Core.Application.Commands.CreateAnOrder;
using DeliveryApp.Core.Application.Commands.CreateCourier;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetAllUncompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;
using Location = OpenApi.Models.Location;

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
        var street = "Айтишная";
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

        var dto = result.Couriers
            .Select(x => new OpenApi.Models.Courier
            {
                Id = x.Id,
                Name = x.Name,
                Location = new Location
                {
                    X = x.Location.X,
                    Y = x.Location.Y,
                }
            })
            .ToList();
        return Ok(dto);
    }

    public override async Task<IActionResult> GetOrders()
    {
        var query = new GetUncompletedOrdersQuery();
        var result = await mediator.Send(query);

        var dto = result.Orders
            .Select(x => new OpenApi.Models.Order
            {
                Id = x.Id,
                Location = new Location
                {
                    X = x.LocationDto.X,
                    Y = x.LocationDto.Y,
                }
            })
            .ToList();
        return Ok(dto);
    }
}