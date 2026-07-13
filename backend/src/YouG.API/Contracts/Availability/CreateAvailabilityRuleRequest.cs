using YouG.Domain.Enums;

namespace YouG.API.Contracts.Availability;

public record CreateAvailabilityRuleRequest(
    DayOfWeek DayOfWeek,
    Daypart Daypart,
    AvailabilityStatus Status,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveUntil);
