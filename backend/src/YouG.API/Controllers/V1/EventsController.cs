using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Events;
using YouG.Application.Features.Events.Commands.CancelEvent;
using YouG.Application.Features.Events.Commands.ConfirmEvent;
using YouG.Application.Features.Events.Commands.ProposeLocationOption;
using YouG.Application.Features.Events.Commands.ProposeTimeOption;
using YouG.Application.Features.Events.Commands.RetractLocationVote;
using YouG.Application.Features.Events.Commands.RetractTimeVote;
using YouG.Application.Features.Events.Commands.SetAttendance;
using YouG.Application.Features.Events.Commands.UpdateEvent;
using YouG.Application.Features.Events.Commands.VoteLocationOption;
using YouG.Application.Features.Events.Commands.VoteTimeOption;
using YouG.Application.Features.Events.Dtos;
using YouG.Application.Features.Events.Queries.GetEventById;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/events")]
public class EventsController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType<EventDetailDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<EventDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEventByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<EventDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<EventDto>> Update(Guid id, UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateEventCommand(id, request.Title, request.Description, request.MaxAttendees), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelEventCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/time-options")]
    [ProducesResponseType<EventTimeOptionDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<EventTimeOptionDto>> ProposeTimeOption(
        Guid id, ProposeTimeOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ProposeTimeOptionCommand(id, request.StartUtc, request.EndUtc), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}/time-options/{optionId:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> VoteTimeOption(Guid id, Guid optionId, CancellationToken cancellationToken)
    {
        await sender.Send(new VoteTimeOptionCommand(id, optionId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/time-options/{optionId:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RetractTimeVote(Guid id, Guid optionId, CancellationToken cancellationToken)
    {
        await sender.Send(new RetractTimeVoteCommand(id, optionId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/location-options")]
    [ProducesResponseType<EventLocationOptionDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<EventLocationOptionDto>> ProposeLocationOption(
        Guid id, ProposeLocationOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ProposeLocationOptionCommand(id, request.Name, request.Address, request.Latitude, request.Longitude),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}/location-options/{optionId:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> VoteLocationOption(Guid id, Guid optionId, CancellationToken cancellationToken)
    {
        await sender.Send(new VoteLocationOptionCommand(id, optionId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/location-options/{optionId:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RetractLocationVote(Guid id, Guid optionId, CancellationToken cancellationToken)
    {
        await sender.Send(new RetractLocationVoteCommand(id, optionId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType<EventDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<EventDto>> Confirm(Guid id, ConfirmEventRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ConfirmEventCommand(id, request.TimeOptionId, request.LocationOptionId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/attendance")]
    [ProducesResponseType<EventAttendanceDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<EventAttendanceDto>> SetAttendance(Guid id, SetAttendanceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetAttendanceCommand(id, request.Status), cancellationToken);
        return Ok(result);
    }
}
