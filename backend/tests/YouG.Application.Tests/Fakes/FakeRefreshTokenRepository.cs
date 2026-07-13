using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    public List<RefreshToken> Tokens { get; } = [];

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        Task.FromResult(Tokens.FirstOrDefault(t => t.TokenHash == tokenHash));

    public void Add(RefreshToken refreshToken) => Tokens.Add(refreshToken);
}
