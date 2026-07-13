using MediatR;

namespace YouG.Application.Features.Events.Commands.RetractTimeVote;

public record RetractTimeVoteCommand(Guid EventId, Guid OptionId) : IRequest;
