using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Events;
using YouG.API.Contracts.Groups;
using YouG.Application.Features.Availability.Dtos;
using YouG.Application.Features.Availability.Queries.GetGroupHeatmap;
using YouG.Application.Features.Availability.Queries.GetGroupOverlap;
using YouG.Application.Features.Events.Commands.CreateEvent;
using YouG.Application.Features.Events.Dtos;
using YouG.Application.Features.Events.Queries.GetGroupEvents;
using YouG.Application.Features.Groups.Commands.ChangeMemberRole;
using YouG.Application.Features.Groups.Commands.CreateGroup;
using YouG.Application.Features.Groups.Commands.CreateInviteLink;
using YouG.Application.Features.Groups.Commands.JoinGroupViaInvite;
using YouG.Application.Features.Groups.Commands.LeaveGroup;
using YouG.Application.Features.Groups.Commands.RemoveMember;
using YouG.Application.Features.Groups.Commands.UpdateGroup;
using YouG.Application.Features.Groups.Dtos;
using YouG.Application.Features.Groups.Queries.GetGroupById;
using YouG.Application.Features.Groups.Queries.GetGroupMembers;
using YouG.Application.Features.Groups.Queries.GetMyGroups;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
public class GroupsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<GroupDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<GroupDto>> Create(CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateGroupCommand(request.Name, request.Description), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    [ProducesResponseType<List<GroupDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GroupDto>>> GetMyGroups(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyGroupsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GroupDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GroupDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGroupByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<GroupDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GroupDto>> Update(Guid id, UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateGroupCommand(id, request.Name, request.Description), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/members")]
    [ProducesResponseType<List<GroupMemberDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GroupMemberDto>>> GetMembers(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGroupMembersQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/members/me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Leave(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new LeaveGroupCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveMemberCommand(id, userId), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeMemberRole(
        Guid id, Guid userId, ChangeMemberRoleRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ChangeMemberRoleCommand(id, userId, request.Role), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/invite-links")]
    [ProducesResponseType<InviteLinkDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<InviteLinkDto>> CreateInviteLink(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateInviteLinkCommand(id), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("join/{code}")]
    [ProducesResponseType<GroupDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GroupDto>> Join(string code, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new JoinGroupViaInviteCommand(code), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/overlap")]
    [ProducesResponseType<OverlapResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<OverlapResultDto>> GetOverlap(
        Guid id,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] bool weekendOnly,
        [FromQuery] string? preferredDayparts,
        CancellationToken cancellationToken)
    {
        var dayparts = string.IsNullOrWhiteSpace(preferredDayparts)
            ? null
            : preferredDayparts.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Enum.Parse<Domain.Enums.Daypart>)
                .ToList();

        var result = await sender.Send(new GetGroupOverlapQuery(id, from, to, weekendOnly, dayparts), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/heatmap")]
    [ProducesResponseType<HeatmapResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<HeatmapResultDto>> GetHeatmap(
        Guid id, [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGroupHeatmapQuery(id, from, to), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/events")]
    [ProducesResponseType<EventDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<EventDto>> CreateEvent(Guid id, CreateEventRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateEventCommand(
                id, request.Title, request.Description, request.MaxAttendees,
                request.InitialStartUtc, request.InitialEndUtc,
                request.InitialLocationName, request.InitialLocationAddress,
                request.InitialLocationLatitude, request.InitialLocationLongitude),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("{id:guid}/events")]
    [ProducesResponseType<List<EventDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventDto>>> GetEvents(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGroupEventsQuery(id), cancellationToken);
        return Ok(result);
    }
}
