using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class GroupInviteLink : Entity
{
    public required Guid GroupId { get; set; }
    public required string Code { get; set; }
    public required Guid CreatedByUserId { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
