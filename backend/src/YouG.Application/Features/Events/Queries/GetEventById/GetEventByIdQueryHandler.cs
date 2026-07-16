using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Queries.GetEventById;

public class GetEventByIdQueryHandler(
    IEventRepository eventRepository,
    IEventTimeOptionRepository timeOptionRepository,
    IEventLocationOptionRepository locationOptionRepository,
    IEventTimeVoteRepository timeVoteRepository,
    IEventLocationVoteRepository locationVoteRepository,
    IEventAttendanceRepository attendanceRepository,
    IGroupMemberRepository groupMemberRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetEventByIdQuery, EventDetailDto>
{
    public async Task<EventDetailDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        // Votes/attendance aren't cleaned up when a member is removed from the group (see
        // RemoveMemberCommandHandler) - they're tied only to EventId/UserId, not GroupMember,
        // so a removed member's vote could otherwise still count toward confirming a time or
        // location, and they'd still show as "Going". Filtering by current membership here
        // keeps the record intact (in case they're re-invited) while not affecting live events.
        var currentMemberIds = (await groupMemberRepository.GetMembersByGroupIdAsync(@event.GroupId, cancellationToken))
            .Select(m => m.UserId)
            .ToHashSet();

        var timeOptions = await timeOptionRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        var timeVotes = (await timeVoteRepository.GetByOptionIdsAsync(timeOptions.Select(o => o.Id).ToList(), cancellationToken))
            .Where(v => currentMemberIds.Contains(v.UserId))
            .ToList();
        var timeOptionDtos = timeOptions.Select(o =>
        {
            var votesForOption = timeVotes.Where(v => v.EventTimeOptionId == o.Id).ToList();
            return new EventTimeOptionDto(
                o.Id, o.StartUtc, o.EndUtc, o.ProposedByUserId,
                votesForOption.Count, votesForOption.Any(v => v.UserId == currentUser.UserId));
        }).ToList();

        var locationOptions = await locationOptionRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        var locationVotes = (await locationVoteRepository.GetByOptionIdsAsync(locationOptions.Select(o => o.Id).ToList(), cancellationToken))
            .Where(v => currentMemberIds.Contains(v.UserId))
            .ToList();
        var locationOptionDtos = locationOptions.Select(o =>
        {
            var votesForOption = locationVotes.Where(v => v.EventLocationOptionId == o.Id).ToList();
            return new EventLocationOptionDto(
                o.Id, o.Name, o.Address, o.Latitude, o.Longitude, o.ProposedByUserId,
                votesForOption.Count, votesForOption.Any(v => v.UserId == currentUser.UserId));
        }).ToList();

        var attendance = await attendanceRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        var attendanceDtos = attendance
            .Where(a => currentMemberIds.Contains(a.UserId))
            .Select(a => new EventAttendanceDto(a.UserId, a.Status, a.RespondedAt))
            .ToList();

        var eventDto = new EventDto(
            @event.Id, @event.GroupId, @event.CreatedByUserId, @event.Title, @event.Description,
            @event.MaxAttendees, @event.Status, @event.ConfirmedTimeOptionId, @event.ConfirmedLocationOptionId, @event.CreatedAt);

        return new EventDetailDto(eventDto, timeOptionDtos, locationOptionDtos, attendanceDtos);
    }
}
