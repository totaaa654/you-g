using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Commands.ConfirmEvent;

public record ConfirmEventCommand(Guid EventId, Guid TimeOptionId, Guid LocationOptionId) : IRequest<EventDto>;
