using FluentValidation;
using YouG.Application.Common;

namespace YouG.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Bio).MaximumLength(500);

        RuleFor(x => x.TimeZoneId)
            .NotEmpty()
            .Must(TimeZoneValidation.IsValidIanaTimeZone)
            .WithMessage("TimeZoneId must be a valid IANA time zone identifier.");
    }
}
