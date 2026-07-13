using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Queries.GetEventById;

public record GetEventByIdQuery(Guid EventId) : IRequest<EventDetailDto>;
