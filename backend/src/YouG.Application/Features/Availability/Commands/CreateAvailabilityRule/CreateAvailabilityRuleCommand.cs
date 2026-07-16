using MediatR;
using YouG.Application.Features.Availability.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Commands.CreateAvailabilityRule;

public record CreateAvailabilityRuleCommand(
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    AvailabilityStatus Status,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveUntil) : IRequest<AvailabilityRuleDto>;
