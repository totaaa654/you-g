using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

public class DeviceToken : Entity
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required DevicePlatform Platform { get; set; }
    public DateTimeOffset LastUsedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
