using MediatR;

namespace YouG.Application.Features.Events.Commands.VoteTimeOption;

public record VoteTimeOptionCommand(Guid EventId, Guid OptionId) : IRequest;
