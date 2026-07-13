using MediatR;
using YouG.Application.Features.Availability.Dtos;

namespace YouG.Application.Features.Availability.Queries.GetGroupHeatmap;

public record GetGroupHeatmapQuery(Guid GroupId, DateOnly From, DateOnly To) : IRequest<HeatmapResultDto>;
