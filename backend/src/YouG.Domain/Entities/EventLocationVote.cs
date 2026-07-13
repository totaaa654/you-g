using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class EventLocationVote : Entity
{
    public required Guid EventLocationOptionId { get; set; }
    public required Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
