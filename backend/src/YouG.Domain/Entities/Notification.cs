using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

public class Notification : Entity
{
    public required Guid UserId { get; set; }
    public required NotificationType Type { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }

    /// <summary>Polymorphic per-type JSON payload (e.g. { "groupId": "...", "eventId": "..." }).</summary>
    public required string Payload { get; set; }

    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
