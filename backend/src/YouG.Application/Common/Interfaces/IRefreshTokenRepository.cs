using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    void Add(RefreshToken refreshToken);
}
