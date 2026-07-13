using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler(
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpdateGroupCommand, GroupDto>
{
    public async Task<GroupDto> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var membership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);
        GroupAuthorization.RequireAdmin(membership);

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Group not found.");

        group.Name = request.Name;
        group.Description = request.Description;
        group.UpdatedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var memberCount = await groupMemberRepository.CountMembersAsync(request.GroupId, cancellationToken);
        return new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, memberCount, group.CreatedAt);
    }
}
