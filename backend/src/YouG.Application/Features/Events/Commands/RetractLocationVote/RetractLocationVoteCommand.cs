using MediatR;

namespace YouG.Application.Features.Events.Commands.RetractLocationVote;

public record RetractLocationVoteCommand(Guid EventId, Guid OptionId) : IRequest;
