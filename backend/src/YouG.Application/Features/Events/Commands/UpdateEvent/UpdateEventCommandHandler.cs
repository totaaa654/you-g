using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler(
    IEventRepository eventRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpdateEventCommand, EventDto>
{
    public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);
        EventAuthorization.RequireOrganizer(@event, currentUser.UserId);

        @event.Title = request.Title;
        @event.Description = request.Description;
        @event.MaxAttendees = request.MaxAttendees;
        @event.UpdatedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EventDto(
            @event.Id, @event.GroupId, @event.CreatedByUserId, @event.Title, @event.Description,
            @event.MaxAttendees, @event.Status, @event.ConfirmedTimeOptionId, @event.ConfirmedLocationOptionId, @event.CreatedAt);
    }
}
