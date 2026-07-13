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

        var timeOptions = await timeOptionRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        var timeVotes = await timeVoteRepository.GetByOptionIdsAsync(timeOptions.Select(o => o.Id).ToList(), cancellationToken);
        var timeOptionDtos = timeOptions.Select(o =>
        {
            var votesForOption = timeVotes.Where(v => v.EventTimeOptionId == o.Id).ToList();
            return new EventTimeOptionDto(
                o.Id, o.StartUtc, o.EndUtc, o.ProposedByUserId,
                votesForOption.Count, votesForOption.Any(v => v.UserId == currentUser.UserId));
        }).ToList();

        var locationOptions = await locationOptionRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        var locationVotes = await locationVoteRepository.GetByOptionIdsAsync(locationOptions.Select(o => o.Id).ToList(), cancellationToken);
        var locationOptionDtos = locationOptions.Select(o =>
        {
            var votesForOption = locationVotes.Where(v => v.EventLocationOptionId == o.Id).ToList();
            return new EventLocationOptionDto(
                o.Id, o.Name, o.Address, o.Latitude, o.Longitude, o.ProposedByUserId,
                votesForOption.Count, votesForOption.Any(v => v.UserId == currentUser.UserId));
        }).ToList();

        var attendance = await attendanceRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        var attendanceDtos = attendance.Select(a => new EventAttendanceDto(a.UserId, a.Status, a.RespondedAt)).ToList();

        var eventDto = new EventDto(
            @event.Id, @event.GroupId, @event.CreatedByUserId, @event.Title, @event.Description,
            @event.MaxAttendees, @event.Status, @event.ConfirmedTimeOptionId, @event.ConfirmedLocationOptionId, @event.CreatedAt);

        return new EventDetailDto(eventDto, timeOptionDtos, locationOptionDtos, attendanceDtos);
    }
}
