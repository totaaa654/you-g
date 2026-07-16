using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Friends;
using YouG.Application.Features.Groups.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Queries.GetGroupJoinRequests;

public class GetGroupJoinRequestsQueryHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupJoinRequestRepository joinRequestRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetGroupJoinRequestsQuery, List<GroupJoinRequestDto>>
{
    public async Task<List<GroupJoinRequestDto>> Handle(GetGroupJoinRequestsQuery request, CancellationToken cancellationToken)
    {
        var membership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);
        GroupAuthorization.RequireAdmin(membership);

        var pending = await joinRequestRepository.GetByStatusForGroupAsync(
            request.GroupId, GroupJoinRequestStatus.Pending, cancellationToken);

        var usersById = (await userRepository.GetByIdsAsync(pending.Select(r => r.UserId).ToList(), cancellationToken))
            .ToDictionary(u => u.Id);

        return pending
            .Where(r => usersById.ContainsKey(r.UserId))
            .Select(r => new GroupJoinRequestDto(r.Id, usersById[r.UserId].ToPublicProfileDto(), r.CreatedAt))
            .ToList();
    }
}
