using MediatR;
using YouG.Application.Features.Availability.Dtos;

namespace YouG.Application.Features.Availability.Queries.GetMyAvailabilityRules;

public record GetMyAvailabilityRulesQuery : IRequest<List<AvailabilityRuleDto>>;
