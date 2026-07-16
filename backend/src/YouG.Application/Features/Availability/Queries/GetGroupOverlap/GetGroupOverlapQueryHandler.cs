using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Dtos;
using YouG.Application.Features.Groups;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Queries.GetGroupOverlap;

public class GetGroupOverlapQueryHandler(
    IGroupMemberRepository groupMemberRepository,
    IAvailabilityInstanceRepository instanceRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetGroupOverlapQuery, OverlapResultDto>
{
    public async Task<OverlapResultDto> Handle(GetGroupOverlapQuery request, CancellationToken cancellationToken)
    {
        await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        var members = await groupMemberRepository.GetMembersByGroupIdAsync(request.GroupId, cancellationToken);
        var memberIds = members.Select(m => m.UserId).ToList();
        var totalMembers = memberIds.Count;

        var instances = await instanceRepository.GetForUsersInRangeAsync(
            memberIds, request.From, request.To, cancellationToken);

        var windows = instances
            .Where(i => i.Status is AvailabilityStatus.Available or AvailabilityStatus.Maybe)
            .Where(i => !request.WeekendOnly || i.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            .GroupBy(i => (i.Date, i.StartTime))
            .Select(g =>
            {
                var availableUserIds = g.Where(i => i.Status == AvailabilityStatus.Available).Select(i => i.UserId).ToList();
                var maybeUserIds = g.Where(i => i.Status == AvailabilityStatus.Maybe).Select(i => i.UserId).ToList();

                return new OverlapWindowDto(
                    g.Key.Date, g.Key.StartTime, availableUserIds, availableUserIds.Count, totalMembers, maybeUserIds);
            })
            // Ranked by overlap size descending (FR-3) — the client renders in this order rather
            // than re-sorting (docs/04-API-DESIGN.md Section 3.2).
            .OrderByDescending(w => w.AvailableCount)
            .ThenBy(w => w.Date)
            .ThenBy(w => w.StartTime)
            .ToList();

        return new OverlapResultDto(request.GroupId, windows);
    }
}
