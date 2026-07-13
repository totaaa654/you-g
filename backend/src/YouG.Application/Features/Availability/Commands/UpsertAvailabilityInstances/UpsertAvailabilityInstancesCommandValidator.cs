using FluentValidation;

namespace YouG.Application.Features.Availability.Commands.UpsertAvailabilityInstances;

public class UpsertAvailabilityInstancesCommandValidator : AbstractValidator<UpsertAvailabilityInstancesCommand>
{
    public UpsertAvailabilityInstancesCommandValidator()
    {
        RuleFor(x => x.Instances).NotEmpty();

        RuleForEach(x => x.Instances).ChildRules(instance =>
        {
            instance.RuleFor(i => i.Daypart).IsInEnum();
            instance.RuleFor(i => i.Status).IsInEnum();
        });
    }
}
