using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;
using YouG.Application.Features.Groups;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Commands.CreateEvent;

public class CreateEventCommandHandler(
    IEventRepository eventRepository,
    IEventTimeOptionRepository timeOptionRepository,
    IEventLocationOptionRepository locationOptionRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CreateEventCommand, EventDto>
{
    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        var now = dateTimeProvider.UtcNow;

        var @event = new Event
        {
            GroupId = request.GroupId,
            CreatedByUserId = currentUser.UserId,
            Title = request.Title,
            Description = request.Description,
            MaxAttendees = request.MaxAttendees,
            Status = EventStatus.Proposed,
            CreatedAt = now,
            UpdatedAt = now
        };
        eventRepository.Add(@event);

        EventTimeOption? timeOption = null;
        if (request.InitialStartUtc.HasValue && request.InitialEndUtc.HasValue)
        {
            timeOption = new EventTimeOption
            {
                EventId = @event.Id,
                StartUtc = request.InitialStartUtc.Value,
                EndUtc = request.InitialEndUtc.Value,
                ProposedByUserId = currentUser.UserId,
                CreatedAt = now
            };
            timeOptionRepository.Add(timeOption);
        }

        EventLocationOption? locationOption = null;
        if (request.InitialLocationName is not null)
        {
            locationOption = new EventLocationOption
            {
                EventId = @event.Id,
                Name = request.InitialLocationName,
                Address = request.InitialLocationAddress,
                Latitude = request.InitialLocationLatitude!.Value,
                Longitude = request.InitialLocationLongitude!.Value,
                ProposedByUserId = currentUser.UserId,
                CreatedAt = now
            };
            locationOptionRepository.Add(locationOption);
        }

        // First save inserts Event + options with ConfirmedTimeOptionId/ConfirmedLocationOptionId
        // still null. Setting those FKs before this insert creates a genuine circular dependency
        // for EF's insert ordering (Event needs the options' EventId FK satisfied, but the options
        // need Event.Id to exist first) — splitting into two saves breaks the cycle.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Both fixed time and location given — the organizer's choice is the confirmed choice,
        // no voting required (docs/03-DATABASE.md Section 3.10 design note).
        if (timeOption is not null && locationOption is not null)
        {
            @event.ConfirmedTimeOptionId = timeOption.Id;
            @event.ConfirmedLocationOptionId = locationOption.Id;
            @event.Status = EventStatus.Confirmed;

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new EventDto(
            @event.Id, @event.GroupId, @event.CreatedByUserId, @event.Title, @event.Description,
            @event.MaxAttendees, @event.Status, @event.ConfirmedTimeOptionId, @event.ConfirmedLocationOptionId, @event.CreatedAt);
    }
}
