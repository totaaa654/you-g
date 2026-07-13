using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Dtos;
using YouG.Application.Features.Groups;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Queries.GetGroupHeatmap;

public class GetGroupHeatmapQueryHandler(
    IGroupMemberRepository groupMemberRepository,
    IAvailabilityInstanceRepository instanceRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetGroupHeatmapQuery, HeatmapResultDto>
{
    public async Task<HeatmapResultDto> Handle(GetGroupHeatmapQuery request, CancellationToken cancellationToken)
    {
        await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        var members = await groupMemberRepository.GetMembersByGroupIdAsync(request.GroupId, cancellationToken);
        var memberIds = members.Select(m => m.UserId).ToList();

        var instances = await instanceRepository.GetForUsersInRangeAsync(
            memberIds, request.From, request.To, cancellationToken);

        // Flat cell list, not a nested date->daypart object — maps directly onto a Flutter
        // GridView with no client-side reshape (docs/04-API-DESIGN.md Section 3.3).
        var cells = instances
            .Where(i => i.Status == AvailabilityStatus.Available)
            .GroupBy(i => (i.Date, i.Daypart))
            .Select(g => new HeatmapCellDto(g.Key.Date, g.Key.Daypart, g.Count()))
            .OrderBy(c => c.Date)
            .ThenBy(c => c.Daypart)
            .ToList();

        return new HeatmapResultDto(request.GroupId, request.From, request.To, cells, memberIds.Count);
    }
}
