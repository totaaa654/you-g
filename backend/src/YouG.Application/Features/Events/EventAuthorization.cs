using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Events;

/// <summary>
/// Shared event-access checks — mirrors GroupAuthorization's 404-for-non-member,
/// 403-for-insufficient-permission pattern (docs/04-API-DESIGN.md Section 4).
/// </summary>
internal static class EventAuthorization
{
    public static async Task<Event> RequireGroupMembershipAsync(
        IEventRepository eventRepository,
        IGroupMemberRepository groupMemberRepository,
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new NotFoundException("Event not found.");

        await GroupAuthorization.RequireMembershipAsync(groupMemberRepository, @event.GroupId, userId, cancellationToken);

        return @event;
    }

    public static void RequireOrganizer(Event @event, Guid userId)
    {
        if (@event.CreatedByUserId != userId)
        {
            throw new ForbiddenException("Only the event organizer can perform this action.");
        }
    }
}
