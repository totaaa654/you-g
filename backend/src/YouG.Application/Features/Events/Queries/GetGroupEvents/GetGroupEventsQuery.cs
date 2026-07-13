using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Queries.GetGroupEvents;

public record GetGroupEventsQuery(Guid GroupId) : IRequest<List<EventDto>>;
