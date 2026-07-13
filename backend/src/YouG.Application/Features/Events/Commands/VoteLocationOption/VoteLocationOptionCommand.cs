using MediatR;

namespace YouG.Application.Features.Events.Commands.VoteLocationOption;

public record VoteLocationOptionCommand(Guid EventId, Guid OptionId) : IRequest;
