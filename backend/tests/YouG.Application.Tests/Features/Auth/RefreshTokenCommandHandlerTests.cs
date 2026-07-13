using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Auth.Commands.Refresh;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Auth;

public class RefreshTokenCommandHandlerTests
{
    private static (RefreshTokenCommandHandler Handler, FakeUserRepository Users, FakeRefreshTokenRepository Tokens, FakeDateTimeProvider Clock)
        CreateHandler(DateTimeOffset now)
    {
        var users = new FakeUserRepository();
        var tokens = new FakeRefreshTokenRepository();
        var clock = new FakeDateTimeProvider(now);
        var handler = new RefreshTokenCommandHandler(users, tokens, new FakeUnitOfWork(), new FakeTokenService(), clock);
        return (handler, users, tokens, clock);
    }

    [Fact]
    public async Task Handle_WithValidToken_RotatesAndReturnsNewTokens()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, users, tokens, _) = CreateHandler(now);

        var user = new User { Email = "a@b.com", Username = "maya22", FriendCode = "YG-000000", DisplayName = "Maya", TimeZoneId = "UTC" };
        users.Users.Add(user);

        var rawToken = "existing-raw-refresh-token";
        var existing = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = $"hash-of:{rawToken}",
            ExpiresAt = now.AddDays(10),
            CreatedAt = now.AddDays(-1)
        };
        tokens.Tokens.Add(existing);

        var result = await handler.Handle(new RefreshTokenCommand(rawToken), CancellationToken.None);

        Assert.NotEmpty(result.AccessToken);
        Assert.NotEqual(rawToken, result.RefreshToken);
        Assert.NotNull(existing.RevokedAt);
        Assert.NotNull(existing.ReplacedByTokenId);
        Assert.Equal(2, tokens.Tokens.Count);
    }

    [Fact]
    public async Task Handle_WithUnknownToken_ThrowsAuthenticationFailedException()
    {
        var (handler, _, _, _) = CreateHandler(DateTimeOffset.UtcNow);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new RefreshTokenCommand("never-issued"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ThrowsAuthenticationFailedException()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, users, tokens, _) = CreateHandler(now);

        var user = new User { Email = "a@b.com", Username = "maya22", FriendCode = "YG-000000", DisplayName = "Maya", TimeZoneId = "UTC" };
        users.Users.Add(user);

        var rawToken = "existing-raw-refresh-token";
        tokens.Tokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = $"hash-of:{rawToken}",
            ExpiresAt = now.AddDays(-1), // already expired
            CreatedAt = now.AddDays(-31)
        });

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new RefreshTokenCommand(rawToken), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyRevokedToken_ThrowsAuthenticationFailedException()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, users, tokens, _) = CreateHandler(now);

        var user = new User { Email = "a@b.com", Username = "maya22", FriendCode = "YG-000000", DisplayName = "Maya", TimeZoneId = "UTC" };
        users.Users.Add(user);

        var rawToken = "existing-raw-refresh-token";
        tokens.Tokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = $"hash-of:{rawToken}",
            ExpiresAt = now.AddDays(10),
            CreatedAt = now.AddDays(-1),
            RevokedAt = now.AddMinutes(-5) // simulates replay of an already-rotated token
        });

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new RefreshTokenCommand(rawToken), CancellationToken.None));
    }
}
