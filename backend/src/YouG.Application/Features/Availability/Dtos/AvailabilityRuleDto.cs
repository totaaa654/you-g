using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Dtos;

public record AvailabilityRuleDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    AvailabilityStatus Status,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveUntil);
