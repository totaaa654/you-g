using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Commands.ProposeTimeOption;

public record ProposeTimeOptionCommand(Guid EventId, DateTimeOffset StartUtc, DateTimeOffset EndUtc) : IRequest<EventTimeOptionDto>;
