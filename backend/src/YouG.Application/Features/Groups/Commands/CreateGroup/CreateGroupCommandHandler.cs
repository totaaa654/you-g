using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups.Dtos;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandHandler(
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CreateGroupCommand, GroupDto>
{
    public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;

        var group = new Group
        {
            Name = request.Name,
            Description = request.Description,
            CreatedByUserId = currentUser.UserId,
            CreatedAt = now,
            UpdatedAt = now
        };

        groupRepository.Add(group);

        // The creator is automatically the first admin — a group can't exist with zero admins.
        groupMemberRepository.Add(new GroupMember
        {
            GroupId = group.Id,
            UserId = currentUser.UserId,
            Role = GroupRole.Admin,
            JoinedAt = now
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, 1, group.CreatedAt);
    }
}
