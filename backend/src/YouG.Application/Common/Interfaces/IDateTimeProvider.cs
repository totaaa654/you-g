namespace YouG.Application.Common.Interfaces;

/// <summary>Testable clock — handlers never call DateTimeOffset.UtcNow directly.</summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
