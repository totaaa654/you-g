using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

/// <summary>
/// A user-edited recurrence definition. Expanded into <see cref="AvailabilityInstance"/> rows
/// by the recurrence materialization background job (see docs/02-ARCHITECTURE.md Section 5.2).
/// </summary>
public class AvailabilityRule : Entity
{
    public required Guid UserId { get; set; }
    public required DayOfWeek DayOfWeek { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required AvailabilityStatus Status { get; set; }
    public required DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveUntil { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
