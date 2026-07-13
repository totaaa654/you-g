namespace YouG.Application.Common.Interfaces;

/// <summary>Commits changes tracked by repositories in this request. Paired with the Repository pattern (docs/02-ARCHITECTURE.md Section 3).</summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
