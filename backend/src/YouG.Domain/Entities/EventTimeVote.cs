using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class EventTimeVote : Entity
{
    public required Guid EventTimeOptionId { get; set; }
    public required Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
