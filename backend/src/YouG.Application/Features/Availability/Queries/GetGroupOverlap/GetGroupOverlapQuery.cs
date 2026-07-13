using MediatR;
using YouG.Application.Features.Availability.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Queries.GetGroupOverlap;

/// <summary>
/// Dayparts (not clock times) filter "preferred time of day" — matches the daypart-granularity
/// decision in docs/01-PRD.md Section 8; there's no hourly data to filter on.
/// </summary>
public record GetGroupOverlapQuery(
    Guid GroupId,
    DateOnly From,
    DateOnly To,
    bool WeekendOnly,
    List<Daypart>? PreferredDayparts) : IRequest<OverlapResultDto>;
