using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups;

/// <summary>
/// Shared membership/admin checks for Group handlers — matches the authorization matrix in
/// docs/04-API-DESIGN.md Section 4 (404 for non-members, 403 for members-but-not-admin).
/// </summary>
internal static class GroupAuthorization
{
    public static async Task<GroupMember> RequireMembershipAsync(
        IGroupMemberRepository groupMemberRepository, Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var member = await groupMemberRepository.GetByGroupAndUserAsync(groupId, userId, cancellationToken);

        // 404, not 403 — a non-member shouldn't be able to distinguish "group doesn't exist"
        // from "group exists but I can't see it."
        return member ?? throw new NotFoundException("Group not found.");
    }

    public static void RequireAdmin(GroupMember member)
    {
        if (member.Role != GroupRole.Admin)
        {
            throw new ForbiddenException("Only group admins can perform this action.");
        }
    }
}
