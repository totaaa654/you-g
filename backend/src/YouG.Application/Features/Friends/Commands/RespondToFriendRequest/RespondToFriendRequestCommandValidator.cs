using FluentValidation;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Commands.RespondToFriendRequest;

public class RespondToFriendRequestCommandValidator : AbstractValidator<RespondToFriendRequestCommand>
{
    public RespondToFriendRequestCommandValidator()
    {
        RuleFor(x => x.Status).Must(s => s is FriendRequestStatus.Accepted or FriendRequestStatus.Declined)
            .WithMessage("Status must be Accepted or Declined.");
    }
}
