using MediatR;

namespace YouG.Application.Features.Events.Commands.CancelEvent;

public record CancelEventCommand(Guid EventId) : IRequest;
