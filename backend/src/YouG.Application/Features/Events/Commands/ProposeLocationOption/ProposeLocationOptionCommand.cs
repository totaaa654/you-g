using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Commands.ProposeLocationOption;

public record ProposeLocationOptionCommand(
    Guid EventId, string Name, string? Address, double Latitude, double Longitude) : IRequest<EventLocationOptionDto>;
