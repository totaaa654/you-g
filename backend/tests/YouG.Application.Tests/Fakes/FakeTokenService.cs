using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeTokenService : ITokenService
{
    private int _refreshTokenCounter;

    public (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user) =>
        ($"access-token-for:{user.Id}", DateTimeOffset.UtcNow.AddMinutes(15));

    public string GenerateRefreshToken() => $"raw-refresh-token-{Interlocked.Increment(ref _refreshTokenCounter)}";

    public string HashToken(string rawToken) => $"hash-of:{rawToken}";
}
