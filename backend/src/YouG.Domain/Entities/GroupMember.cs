using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

public class GroupMember : Entity
{
    public required Guid GroupId { get; set; }
    public required Guid UserId { get; set; }
    public GroupRole Role { get; set; } = GroupRole.Member;
    public DateTimeOffset JoinedAt { get; set; }
}
