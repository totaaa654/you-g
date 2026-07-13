using MediatR;

namespace YouG.Application.Features.Availability.Commands.DeleteAvailabilityRule;

public record DeleteAvailabilityRuleCommand(Guid RuleId) : IRequest;
