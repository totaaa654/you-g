using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Commands.UpdateEvent;

public record UpdateEventCommand(Guid EventId, string Title, string? Description, int? MaxAttendees) : IRequest<EventDto>;
