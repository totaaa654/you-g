using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Commands.SetAttendance;

public class SetAttendanceCommandHandler(
    IEventRepository eventRepository,
    IEventAttendanceRepository attendanceRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<SetAttendanceCommand, EventAttendanceDto>
{
    public async Task<EventAttendanceDto> Handle(SetAttendanceCommand request, CancellationToken cancellationToken)
    {
        var @event = await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var existing = await attendanceRepository.GetAsync(request.EventId, currentUser.UserId, cancellationToken);
        var now = dateTimeProvider.UtcNow;

        if (request.Status == EventAttendanceStatus.Going &&
            @event.MaxAttendees.HasValue &&
            existing?.Status != EventAttendanceStatus.Going)
        {
            var attendance = await attendanceRepository.GetByEventIdAsync(request.EventId, cancellationToken);
            var goingCount = attendance.Count(a => a.Status == EventAttendanceStatus.Going);

            if (goingCount >= @event.MaxAttendees.Value)
            {
                throw new ConflictException("This event is at capacity.");
            }
        }

        if (existing is null)
        {
            existing = new EventAttendance
            {
                EventId = request.EventId,
                UserId = currentUser.UserId,
                Status = request.Status,
                RespondedAt = now
            };
            attendanceRepository.Add(existing);
        }
        else
        {
            existing.Status = request.Status;
            existing.RespondedAt = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EventAttendanceDto(existing.UserId, existing.Status, existing.RespondedAt);
    }
}
