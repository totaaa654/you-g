using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Dtos;

public record AvailabilityRuleDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    Daypart Daypart,
    AvailabilityStatus Status,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveUntil);
