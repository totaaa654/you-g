using FluentValidation;

namespace YouG.Application.Features.Notifications.Commands.RegisterDeviceToken;

public class RegisterDeviceTokenCommandValidator : AbstractValidator<RegisterDeviceTokenCommand>
{
    public RegisterDeviceTokenCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
