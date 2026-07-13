using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
