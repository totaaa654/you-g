using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>Every currently-active (not revoked, not expired) token for a user — used to force logout everywhere on account deletion.</summary>
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    void Add(RefreshToken refreshToken);
}
