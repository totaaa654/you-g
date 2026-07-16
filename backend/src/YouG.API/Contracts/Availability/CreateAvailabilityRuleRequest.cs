using YouG.Domain.Enums;

namespace YouG.API.Contracts.Availability;

public record CreateAvailabilityRuleRequest(
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    AvailabilityStatus Status,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveUntil);
