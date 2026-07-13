using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;
using YouG.Application.Features.Groups;

namespace YouG.Application.Features.Events.Queries.GetGroupEvents;

public class GetGroupEventsQueryHandler(
    IEventRepository eventRepository,
    IGroupMemberRepository groupMemberRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetGroupEventsQuery, List<EventDto>>
{
    public async Task<List<EventDto>> Handle(GetGroupEventsQuery request, CancellationToken cancellationToken)
    {
        await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        var events = await eventRepository.GetByGroupIdAsync(request.GroupId, cancellationToken);

        return events
            .Select(e => new EventDto(
                e.Id, e.GroupId, e.CreatedByUserId, e.Title, e.Description,
                e.MaxAttendees, e.Status, e.ConfirmedTimeOptionId, e.ConfirmedLocationOptionId, e.CreatedAt))
            .ToList();
    }
}
