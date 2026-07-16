using MediatR;
using YouG.Application.Features.Availability.Dtos;

namespace YouG.Application.Features.Availability.Queries.GetGroupOverlap;

public record GetGroupOverlapQuery(Guid GroupId, DateOnly From, DateOnly To, bool WeekendOnly) : IRequest<OverlapResultDto>;
