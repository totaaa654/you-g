using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Availability.Commands.CreateAvailabilityRule;

public class CreateAvailabilityRuleCommandHandler(
    IAvailabilityRuleRepository ruleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    IRecurrenceMaterializationJob materializationJob) : IRequestHandler<CreateAvailabilityRuleCommand, AvailabilityRuleDto>
{
    public async Task<AvailabilityRuleDto> Handle(CreateAvailabilityRuleCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;

        var rule = new AvailabilityRule
        {
            UserId = currentUser.UserId,
            DayOfWeek = request.DayOfWeek,
            Daypart = request.Daypart,
            Status = request.Status,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveUntil = request.EffectiveUntil,
            CreatedAt = now,
            UpdatedAt = now
        };

        ruleRepository.Add(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Immediate feedback — the user should see instances appear without waiting for the
        // next periodic sweep (docs/02-ARCHITECTURE.md Section 5.2).
        await materializationJob.RunForUserAsync(currentUser.UserId, cancellationToken);

        return new AvailabilityRuleDto(rule.Id, rule.DayOfWeek, rule.Daypart, rule.Status, rule.EffectiveFrom, rule.EffectiveUntil);
    }
}
