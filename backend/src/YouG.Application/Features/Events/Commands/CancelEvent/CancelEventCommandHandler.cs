using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Commands.CancelEvent;

public class CancelEventCommandHandler(
    IEventRepository eventRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CancelEventCommand>
{
    public async Task Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);
        EventAuthorization.RequireOrganizer(@event, currentUser.UserId);

        @event.Status = EventStatus.Cancelled;
        @event.UpdatedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
