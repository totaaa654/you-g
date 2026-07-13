using MediatR;
using YouG.Application.Features.Events.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Commands.SetAttendance;

public record SetAttendanceCommand(Guid EventId, EventAttendanceStatus Status) : IRequest<EventAttendanceDto>;
