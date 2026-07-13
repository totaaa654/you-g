using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

/// <summary>Doubles as the friendship record — Status = Accepted is the friendship itself (see docs/03-DATABASE.md).</summary>
public class FriendRequest : Entity
{
    public required Guid RequesterId { get; set; }
    public required Guid AddresseeId { get; set; }
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
}
