using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class Group : Entity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public required Guid CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
