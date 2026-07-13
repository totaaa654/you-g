using YouG.Application.Common.Interfaces;

namespace YouG.Application.Tests.Fakes;

public class FakeDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;
}
