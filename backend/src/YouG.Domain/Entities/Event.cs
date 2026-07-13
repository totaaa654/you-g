using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

/// <summary>
/// Always resolves its time/location through <see cref="EventTimeOption"/>/<see cref="EventLocationOption"/> —
/// even an organizer-fixed time is modeled as a single pre-confirmed option (see docs/03-DATABASE.md Section 3.10).
/// </summary>
public class Event : Entity
{
    public required Guid GroupId { get; set; }
    public required Guid CreatedByUserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int? MaxAttendees { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Proposed;
    public Guid? ConfirmedTimeOptionId { get; set; }
    public Guid? ConfirmedLocationOptionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
