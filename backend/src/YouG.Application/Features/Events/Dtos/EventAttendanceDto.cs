using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Dtos;

public record EventAttendanceDto(Guid UserId, EventAttendanceStatus Status, DateTimeOffset RespondedAt);
