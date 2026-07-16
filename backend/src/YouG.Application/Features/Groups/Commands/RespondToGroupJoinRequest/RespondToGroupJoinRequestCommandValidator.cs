using FluentValidation;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.RespondToGroupJoinRequest;

public class RespondToGroupJoinRequestCommandValidator : AbstractValidator<RespondToGroupJoinRequestCommand>
{
    public RespondToGroupJoinRequestCommandValidator()
    {
        RuleFor(x => x.Status).Must(s => s is GroupJoinRequestStatus.Accepted or GroupJoinRequestStatus.Declined)
            .WithMessage("Status must be Accepted or Declined.");
    }
}
