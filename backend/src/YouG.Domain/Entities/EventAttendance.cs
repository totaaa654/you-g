using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

public class EventAttendance : Entity
{
    public required Guid EventId { get; set; }
    public required Guid UserId { get; set; }
    public required EventAttendanceStatus Status { get; set; }
    public DateTimeOffset RespondedAt { get; set; }
}
