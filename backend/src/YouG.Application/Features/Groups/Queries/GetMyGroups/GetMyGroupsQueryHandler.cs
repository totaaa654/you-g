using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetMyGroups;

public class GetMyGroupsQueryHandler(
    IGroupMemberRepository groupMemberRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyGroupsQuery, List<GroupDto>>
{
    public async Task<List<GroupDto>> Handle(GetMyGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await groupMemberRepository.GetGroupsForUserAsync(currentUser.UserId, cancellationToken);

        var result = new List<GroupDto>(groups.Count);
        foreach (var group in groups)
        {
            var memberCount = await groupMemberRepository.CountMembersAsync(group.Id, cancellationToken);
            result.Add(new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, memberCount, group.CreatedAt));
        }

        return result;
    }
}
