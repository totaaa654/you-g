namespace YouG.Application.Common.Interfaces;

/// <summary>
/// Expands AvailabilityRules into materialized AvailabilityInstance rows (docs/02-ARCHITECTURE.md
/// Section 5.2). RunForUserAsync gives immediate feedback right after a rule is created/edited;
/// RunFullSweepAsync is what the periodic background service calls to keep the rolling horizon
/// advancing for everyone over time.
/// </summary>
public interface IRecurrenceMaterializationJob
{
    Task RunForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task RunFullSweepAsync(CancellationToken cancellationToken);
}
