using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Dtos;

namespace YouG.Application.Features.Availability.Queries.GetMyAvailabilityRules;

public class GetMyAvailabilityRulesQueryHandler(
    IAvailabilityRuleRepository ruleRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyAvailabilityRulesQuery, List<AvailabilityRuleDto>>
{
    public async Task<List<AvailabilityRuleDto>> Handle(GetMyAvailabilityRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = await ruleRepository.GetByUserIdAsync(currentUser.UserId, cancellationToken);

        return rules
            .Select(r => new AvailabilityRuleDto(r.Id, r.DayOfWeek, r.Daypart, r.Status, r.EffectiveFrom, r.EffectiveUntil))
            .ToList();
    }
}
