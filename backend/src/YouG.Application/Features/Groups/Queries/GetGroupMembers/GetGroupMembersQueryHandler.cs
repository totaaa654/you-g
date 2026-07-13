using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetGroupMembers;

public class GetGroupMembersQueryHandler(
    IGroupMemberRepository groupMemberRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetGroupMembersQuery, List<GroupMemberDto>>
{
    public async Task<List<GroupMemberDto>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
    {
        await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        var members = await groupMemberRepository.GetMembersByGroupIdAsync(request.GroupId, cancellationToken);
        var users = await userRepository.GetByIdsAsync(members.Select(m => m.UserId).ToList(), cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);

        return members
            .Where(m => usersById.ContainsKey(m.UserId))
            .Select(m =>
            {
                var user = usersById[m.UserId];
                return new GroupMemberDto(user.Id, user.Username, user.DisplayName, user.ProfilePictureUrl, m.Role, m.JoinedAt);
            })
            .ToList();
    }
}
