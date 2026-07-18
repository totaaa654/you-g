using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Commands.ConfirmEvent;

public class ConfirmEventCommandHandler(
    IEventRepository eventRepository,
    IEventTimeOptionRepository timeOptionRepository,
    IEventLocationOptionRepository locationOptionRepository,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    INotificationDispatcher notificationDispatcher) : IRequestHandler<ConfirmEventCommand, EventDto>
{
    public async Task<EventDto> Handle(ConfirmEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);
        EventAuthorization.RequireOrganizer(@event, currentUser.UserId);

        if (@event.Status == EventStatus.Cancelled)
        {
            throw new ConflictException("Cannot confirm a cancelled event.");
        }

        var timeOption = await timeOptionRepository.GetByIdAsync(request.TimeOptionId, cancellationToken);
        if (timeOption is null || timeOption.EventId != request.EventId)
        {
            throw new NotFoundException("Time option not found.");
        }

        var locationOption = await locationOptionRepository.GetByIdAsync(request.LocationOptionId, cancellationToken);
        if (locationOption is null || locationOption.EventId != request.EventId)
        {
            throw new NotFoundException("Location option not found.");
        }

        @event.ConfirmedTimeOptionId = timeOption.Id;
        @event.ConfirmedLocationOptionId = locationOption.Id;
        @event.Status = EventStatus.Confirmed;
        @event.UpdatedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var group = await groupRepository.GetByIdAsync(@event.GroupId, cancellationToken);
        if (group is not null)
        {
            var members = await groupMemberRepository.GetMembersByGroupIdAsync(@event.GroupId, cancellationToken);
            foreach (var member in members.Where(m => m.UserId != currentUser.UserId))
            {
                await notificationDispatcher.DispatchAsync(
                    member.UserId, NotificationType.ScheduleUpdate, $"\"{@event.Title}\" is confirmed",
                    $"The time and location for \"{@event.Title}\" in {group.Name} have been locked in.",
                    new Dictionary<string, string> { ["eventId"] = @event.Id.ToString(), ["groupId"] = group.Id.ToString() },
                    cancellationToken);
            }
        }

        return new EventDto(
            @event.Id, @event.GroupId, @event.CreatedByUserId, @event.Title, @event.Description,
            @event.MaxAttendees, @event.Status, @event.ConfirmedTimeOptionId, @event.ConfirmedLocationOptionId, @event.CreatedAt);
    }
}
