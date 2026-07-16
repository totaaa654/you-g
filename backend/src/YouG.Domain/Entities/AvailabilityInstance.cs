using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

/// <summary>
/// Materialized availability for a specific date + 30-minute slot (StartTime to StartTime+30min)
/// — what overlap queries actually read. SourceRuleId is null for one-off manual overrides
/// (see docs/03-DATABASE.md Section 3.9).
/// </summary>
public class AvailabilityInstance : Entity
{
    public required Guid UserId { get; set; }
    public required DateOnly Date { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required AvailabilityStatus Status { get; set; }
    public Guid? SourceRuleId { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
