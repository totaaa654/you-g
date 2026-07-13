using FluentValidation;

namespace YouG.Application.Features.Friends.Commands.SendFriendRequest;

public class SendFriendRequestCommandValidator : AbstractValidator<SendFriendRequestCommand>
{
    public SendFriendRequestCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.AddresseeId.HasValue || !string.IsNullOrWhiteSpace(x.FriendCode))
            .WithMessage("Either addresseeId or friendCode is required.");
    }
}
