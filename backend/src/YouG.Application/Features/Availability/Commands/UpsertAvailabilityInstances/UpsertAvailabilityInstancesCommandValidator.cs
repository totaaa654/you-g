using FluentValidation;

namespace YouG.Application.Features.Availability.Commands.UpsertAvailabilityInstances;

public class UpsertAvailabilityInstancesCommandValidator : AbstractValidator<UpsertAvailabilityInstancesCommand>
{
    public UpsertAvailabilityInstancesCommandValidator()
    {
        RuleFor(x => x.Instances).NotEmpty();

        RuleForEach(x => x.Instances).ChildRules(instance =>
        {
            instance.RuleFor(i => i.StartTime)
                .Must(t => t.Minute is 0 or 30 && t.Second == 0)
                .WithMessage("StartTime must fall on a 30-minute boundary.");
            instance.RuleFor(i => i.Status).IsInEnum();
        });
    }
}
