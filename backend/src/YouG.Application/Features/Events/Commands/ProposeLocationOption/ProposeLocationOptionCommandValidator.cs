using FluentValidation;

namespace YouG.Application.Features.Events.Commands.ProposeLocationOption;

public class ProposeLocationOptionCommandValidator : AbstractValidator<ProposeLocationOptionCommand>
{
    public ProposeLocationOptionCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}
