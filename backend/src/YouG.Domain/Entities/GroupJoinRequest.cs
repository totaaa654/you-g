using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

/// <summary>
/// Created instead of an instant membership when a user joins via an invite link that a
/// non-admin member created — an admin must approve it before the user actually joins.
/// Links created by an admin still join instantly and never produce one of these.
/// </summary>
public class GroupJoinRequest : Entity
{
    public required Guid GroupId { get; set; }
    public required Guid UserId { get; set; }
    public GroupJoinRequestStatus Status { get; set; } = GroupJoinRequestStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
}
