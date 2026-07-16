using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IPasswordResetTokenRepository
{
    /// <summary>Unused tokens for a user, regardless of expiry — the caller decides what "still
    /// valid" means (used both to find the current code and to retire stale ones on a new request).</summary>
    Task<List<PasswordResetToken>> GetPendingForUserAsync(Guid userId, CancellationToken cancellationToken);

    void Add(PasswordResetToken token);
}
