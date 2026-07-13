using YouG.Domain.Common;

namespace YouG.Domain.Entities;

public class EventLocationOption : Entity
{
    public required Guid EventId { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required Guid ProposedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
