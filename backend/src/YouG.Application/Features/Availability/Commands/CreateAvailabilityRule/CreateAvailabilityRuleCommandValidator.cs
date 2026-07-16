using FluentValidation;

namespace YouG.Application.Features.Availability.Commands.CreateAvailabilityRule;

public class CreateAvailabilityRuleCommandValidator : AbstractValidator<CreateAvailabilityRuleCommand>
{
    public CreateAvailabilityRuleCommandValidator()
    {
        RuleFor(x => x.DayOfWeek).IsInEnum();
        RuleFor(x => x.StartTime)
            .Must(t => t.Minute is 0 or 30 && t.Second == 0)
            .WithMessage("StartTime must fall on a 30-minute boundary.");
        RuleFor(x => x.Status)
            .Must(s => s is Domain.Enums.AvailabilityStatus.Available or Domain.Enums.AvailabilityStatus.Busy or Domain.Enums.AvailabilityStatus.Maybe)
            .WithMessage("Status must be Available, Busy, or Maybe — 'Unknown' isn't something you can declare, it's the absence of a declaration.");

        RuleFor(x => x.EffectiveUntil)
            .GreaterThanOrEqualTo(x => x.EffectiveFrom)
            .When(x => x.EffectiveUntil.HasValue)
            .WithMessage("EffectiveUntil must be on or after EffectiveFrom.");
    }
}
