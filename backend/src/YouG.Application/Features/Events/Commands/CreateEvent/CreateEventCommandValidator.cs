using FluentValidation;

namespace YouG.Application.Features.Events.Commands.CreateEvent;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MaxAttendees).GreaterThan(0).When(x => x.MaxAttendees.HasValue);

        RuleFor(x => x.InitialEndUtc)
            .GreaterThan(x => x.InitialStartUtc)
            .When(x => x.InitialStartUtc.HasValue && x.InitialEndUtc.HasValue)
            .WithMessage("InitialEndUtc must be after InitialStartUtc.");

        RuleFor(x => x)
            .Must(x => x.InitialStartUtc.HasValue == x.InitialEndUtc.HasValue)
            .WithMessage("InitialStartUtc and InitialEndUtc must be provided together.");

        RuleFor(x => x)
            .Must(x => (x.InitialLocationName is null) == (x.InitialLocationLatitude is null) &&
                       (x.InitialLocationName is null) == (x.InitialLocationLongitude is null))
            .WithMessage("InitialLocationName, InitialLocationLatitude, and InitialLocationLongitude must all be provided together.");
    }
}
