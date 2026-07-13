using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups.Dtos;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.JoinGroupViaInvite;

public class JoinGroupViaInviteCommandHandler(
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    IGroupInviteLinkRepository inviteLinkRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<JoinGroupViaInviteCommand, GroupDto>
{
    public async Task<GroupDto> Handle(JoinGroupViaInviteCommand request, CancellationToken cancellationToken)
    {
        var inviteLink = await inviteLinkRepository.GetByCodeAsync(request.Code, cancellationToken);
        var now = dateTimeProvider.UtcNow;

        if (inviteLink is null || inviteLink.RevokedAt is not null || inviteLink.ExpiresAt <= now)
        {
            throw new NotFoundException("This invite link is invalid or has expired.");
        }

        var group = await groupRepository.GetByIdAsync(inviteLink.GroupId, cancellationToken)
            ?? throw new NotFoundException("This invite link is invalid or has expired.");

        var existingMembership = await groupMemberRepository.GetByGroupAndUserAsync(
            group.Id, currentUser.UserId, cancellationToken);

        // Re-using a still-valid link is a no-op, not an error — matches the "multi-use" invite
        // link design (docs/01-PRD.md Section 8).
        if (existingMembership is null)
        {
            groupMemberRepository.Add(new GroupMember
            {
                GroupId = group.Id,
                UserId = currentUser.UserId,
                Role = GroupRole.Member,
                JoinedAt = now
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var memberCount = await groupMemberRepository.CountMembersAsync(group.Id, cancellationToken);
        return new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, memberCount, group.CreatedAt);
    }
}
