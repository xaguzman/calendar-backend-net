using System.Linq;
using System.Security.Claims;
using CalendarBackend.Models;
using CalendarBackend.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CalendarBackend.Routes;

// /api/events
internal class EventRoutes
{

    private IResult ValidationErrorsReponse(ValidationResult results)
    {
        var dic = results.ToDictionary();

        // return Results.ValidationProblem(new {
        //     [{status = ""}]
        // });

        return Results.Json(
            new
            {
                errors = dic,
                status = 400
            },
            statusCode: 400
        );
    }

    public async Task<IResult> GetEvents(HttpContext ctx, EventsService eventsService)
    {
        var userId = ctx.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        var events = await eventsService.GetViewAllAsync(x => x.UserId == userId);

        return Results.Json(
            new
            {
                ok = true,
                message = "getEvents",
                events = events
            }
        );
    }

    internal async Task<IResult> CreateEvent(EventModel model, IValidator<EventModel> validator, EventsService eventsService, HttpContext ctx)
    {
        var userId = ctx.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var userName = ctx.User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        model.UserId = userId;
        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            return ValidationErrorsReponse(validationResult);
        }

        await eventsService.CreateAsync(model);

        var resultEvent = new EventModelView(model);
        
        resultEvent.User = new UserDisplayView {
            Id = userId, 
            Name = userName
        };

        return Results.Json(
            new
            {
                ok = true,
                message = "createEvent",
                model = resultEvent
            }
        );
    }

    internal async Task<IResult> DeleteEvent(string id, EventsService eventsService, HttpContext ctx)
    {
        var ev = await eventsService.GetAsync(id);
        var userId = ctx.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        if (ev == null){
            return Results.BadRequest(
                new {
                    ok = false,
                    message = "No existe evento con ese id"
                }
            );
        }

        if ( ev.UserId != userId )
        {
            return Results.BadRequest(
                new {
                    ok = false,
                    message = "No tiene  privilegio de borrar este evento"
                }
            );
        }

        await eventsService.RemoveAsync(id);

        return Results.Json(
            new
            {
                ok = true
            }
        );
    }

    internal async Task<IResult> UpdateEvent(string id, HttpContext ctx, EventModel inputEvent, EventsService eventsService)
    {
        var ev = await eventsService.GetAsync(id);
        var userId = ctx.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var userName = ctx.User.Claims.First(x => x.Type == ClaimTypes.Name).Value;

        if (ev == null){
            return Results.BadRequest(
                new {
                    ok = false,
                    message = "No existe evento con ese id"
                }
            );
        }

        if ( ev.UserId != userId )
        {
            return Results.BadRequest(
                new {
                    ok = false,
                    message = "No tiene  privilegio de editar este evento"
                }
            );
        }

        inputEvent.UserId = userId;
        inputEvent.Id = id;
        var updatedEvent = await eventsService.UpdateAsync(id, inputEvent);

        var resultEvent = new EventModelView(updatedEvent);
        resultEvent.User = new UserDisplayView {
            Id = userId, 
            Name = userName
        };

        return Results.Json(
            new
            {
                ok = true,
                model = resultEvent
            }
        );
    }
}