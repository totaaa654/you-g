using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class EventTimeOption : Entity
{
    public required Guid EventId { get; set; }
    public required DateTimeOffset StartUtc { get; set; }
    public required DateTimeOffset EndUtc { get; set; }
    public required Guid ProposedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
