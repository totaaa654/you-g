using FluentValidation;

namespace YouG.Application.Features.Events.Commands.ProposeTimeOption;

public class ProposeTimeOptionCommandValidator : AbstractValidator<ProposeTimeOptionCommand>
{
    public ProposeTimeOptionCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.EndUtc).GreaterThan(x => x.StartUtc);
    }
}
