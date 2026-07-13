using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetGroupById;

public class GetGroupByIdQueryHandler(
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetGroupByIdQuery, GroupDto>
{
    public async Task<GroupDto> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Group not found.");

        var memberCount = await groupMemberRepository.CountMembersAsync(request.GroupId, cancellationToken);
        return new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, memberCount, group.CreatedAt);
    }
}
