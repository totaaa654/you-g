using YouG.Domain.Enums;

namespace YouG.API.Contracts.Events;

public record SetAttendanceRequest(EventAttendanceStatus Status);
