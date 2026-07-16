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
    IGroupJoinRequestRepository joinRequestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<JoinGroupViaInviteCommand, JoinGroupResultDto>
{
    public async Task<JoinGroupResultDto> Handle(JoinGroupViaInviteCommand request, CancellationToken cancellationToken)
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
        if (existingMembership is not null)
        {
            return await BuildJoinedResultAsync(group, cancellationToken);
        }

        // Only an invite created by a current admin joins instantly. A link created by a regular
        // member (or by someone who has since left/been demoted) routes the joiner through an
        // admin-approved GroupJoinRequest instead — members can share invites, but can't add
        // people to the group unchecked.
        var creatorMembership = await groupMemberRepository.GetByGroupAndUserAsync(
            group.Id, inviteLink.CreatedByUserId, cancellationToken);

        if (creatorMembership is { Role: GroupRole.Admin })
        {
            groupMemberRepository.Add(new GroupMember
            {
                GroupId = group.Id,
                UserId = currentUser.UserId,
                Role = GroupRole.Member,
                JoinedAt = now
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await BuildJoinedResultAsync(group, cancellationToken);
        }

        var existingRequest = await joinRequestRepository.GetByGroupAndUserAsync(
            group.Id, currentUser.UserId, cancellationToken);

        if (existingRequest is null)
        {
            joinRequestRepository.Add(new GroupJoinRequest
            {
                GroupId = group.Id,
                UserId = currentUser.UserId,
                Status = GroupJoinRequestStatus.Pending,
                CreatedAt = now
            });
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (existingRequest.Status == GroupJoinRequestStatus.Declined)
        {
            // Re-using the link after a prior decline re-opens a fresh request rather than
            // permanently locking the user out — matches the FriendRequest re-open pattern.
            existingRequest.Status = GroupJoinRequestStatus.Pending;
            existingRequest.CreatedAt = now;
            existingRequest.RespondedAt = null;
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new JoinGroupResultDto(false, null);
    }

    private async Task<JoinGroupResultDto> BuildJoinedResultAsync(Group group, CancellationToken cancellationToken)
    {
        var memberCount = await groupMemberRepository.CountMembersAsync(group.Id, cancellationToken);
        var dto = new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, memberCount, group.CreatedAt);
        return new JoinGroupResultDto(true, dto);
    }
}
