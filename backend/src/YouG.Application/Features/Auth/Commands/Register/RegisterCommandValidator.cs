using FluentValidation;

namespace YouG.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(32)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username may only contain letters, numbers, and underscores.");

        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.TimeZoneId)
            .NotEmpty()
            .Must(BeAValidTimeZone)
            .WithMessage("TimeZoneId must be a valid IANA time zone identifier.");
    }

    private static bool BeAValidTimeZone(string timeZoneId)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}
