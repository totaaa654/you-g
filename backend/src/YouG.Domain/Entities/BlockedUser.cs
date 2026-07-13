using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class BlockedUser : Entity
{
    public required Guid BlockerId { get; set; }
    public required Guid BlockedId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
