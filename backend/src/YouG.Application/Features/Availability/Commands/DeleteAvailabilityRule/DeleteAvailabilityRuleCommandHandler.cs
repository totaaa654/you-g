using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Availability.Commands.DeleteAvailabilityRule;

public class DeleteAvailabilityRuleCommandHandler(
    IAvailabilityRuleRepository ruleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<DeleteAvailabilityRuleCommand>
{
    public async Task Handle(DeleteAvailabilityRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await ruleRepository.GetByIdAsync(request.RuleId, cancellationToken);

        // Not found, or not yours — both look identical from the outside (never leak that a
        // given rule ID belongs to someone else).
        if (rule is null || rule.UserId != currentUser.UserId)
        {
            throw new NotFoundException("Availability rule not found.");
        }

        ruleRepository.Remove(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
