using YouG.Application.Features.Auth.Commands.Logout;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Auth;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveToken_RevokesIt()
    {
        var tokens = new FakeRefreshTokenRepository();
        var now = DateTimeOffset.UtcNow;
        var rawToken = "raw-refresh-token-1";
        var token = new RefreshToken
        {
            UserId = Guid.CreateVersion7(),
            TokenHash = $"hash-of:{rawToken}",
            ExpiresAt = now.AddDays(10),
            CreatedAt = now
        };
        tokens.Tokens.Add(token);

        var handler = new LogoutCommandHandler(tokens, new FakeUnitOfWork(), new FakeTokenService(), new FakeDateTimeProvider(now));

        await handler.Handle(new LogoutCommand(rawToken), CancellationToken.None);

        Assert.NotNull(token.RevokedAt);
    }

    [Fact]
    public async Task Handle_WithUnknownToken_DoesNotThrow()
    {
        var tokens = new FakeRefreshTokenRepository();
        var handler = new LogoutCommandHandler(tokens, new FakeUnitOfWork(), new FakeTokenService(), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        var exception = await Record.ExceptionAsync(() => handler.Handle(new LogoutCommand("never-issued"), CancellationToken.None));

        Assert.Null(exception);
    }
}
