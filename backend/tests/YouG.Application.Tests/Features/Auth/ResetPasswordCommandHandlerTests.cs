using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Auth.Commands.ResetPassword;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Auth;

public class ResetPasswordCommandHandlerTests
{
    private const string ValidCode = "042817";

    private static ResetPasswordCommandHandler CreateHandler(
        FakeUserRepository userRepository, FakePasswordResetTokenRepository resetTokens,
        FakeRefreshTokenRepository refreshTokens, DateTimeOffset now) =>
        new(
            userRepository,
            resetTokens,
            refreshTokens,
            new FakeUnitOfWork(),
            new FakePasswordHasher(),
            new FakeTokenService(),
            new FakeDateTimeProvider(now));

    private static User ExistingUser() => new()
    {
        Id = Guid.CreateVersion7(),
        Email = "maya@example.com",
        PasswordHash = "hashed:old-password",
        Username = "maya22",
        FriendCode = "YG-000000",
        DisplayName = "Maya",
        TimeZoneId = "Australia/Sydney"
    };

    private static PasswordResetToken PendingToken(Guid userId, DateTimeOffset now, string code = ValidCode) => new()
    {
        UserId = userId, TokenHash = $"hash-of:{code}", ExpiresAt = now.AddMinutes(10), CreatedAt = now
    };

    [Fact]
    public async Task Handle_ValidCode_UpdatesPasswordAndMarksTokenUsed()
    {
        var now = DateTimeOffset.UtcNow;
        var user = ExistingUser();
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(user);

        var resetTokens = new FakePasswordResetTokenRepository();
        var token = PendingToken(user.Id, now);
        resetTokens.Add(token);

        var handler = CreateHandler(userRepository, resetTokens, new FakeRefreshTokenRepository(), now);

        await handler.Handle(new ResetPasswordCommand(user.Email, ValidCode, "new-password-123"), CancellationToken.None);

        Assert.Equal("hashed:new-password-123", user.PasswordHash);
        Assert.NotNull(token.UsedAt);
    }

    [Fact]
    public async Task Handle_ValidCode_RevokesActiveSessions()
    {
        var now = DateTimeOffset.UtcNow;
        var user = ExistingUser();
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(user);

        var resetTokens = new FakePasswordResetTokenRepository();
        resetTokens.Add(PendingToken(user.Id, now));

        var refreshTokens = new FakeRefreshTokenRepository();
        var activeSession = new RefreshToken
        {
            UserId = user.Id, TokenHash = "some-session-hash", ExpiresAt = now.AddDays(30), CreatedAt = now
        };
        refreshTokens.Add(activeSession);

        var handler = CreateHandler(userRepository, resetTokens, refreshTokens, now);

        await handler.Handle(new ResetPasswordCommand(user.Email, ValidCode, "new-password-123"), CancellationToken.None);

        Assert.NotNull(activeSession.RevokedAt);
    }

    [Fact]
    public async Task Handle_ExpiredCode_ThrowsAuthenticationFailedException()
    {
        var now = DateTimeOffset.UtcNow;
        var user = ExistingUser();
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(user);

        var resetTokens = new FakePasswordResetTokenRepository();
        var token = PendingToken(user.Id, now);
        token.ExpiresAt = now.AddMinutes(-1);
        resetTokens.Add(token);

        var handler = CreateHandler(userRepository, resetTokens, new FakeRefreshTokenRepository(), now);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new ResetPasswordCommand(user.Email, ValidCode, "new-password-123"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongCode_ThrowsAndIncrementsFailedAttempts()
    {
        var now = DateTimeOffset.UtcNow;
        var user = ExistingUser();
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(user);

        var resetTokens = new FakePasswordResetTokenRepository();
        var token = PendingToken(user.Id, now);
        resetTokens.Add(token);

        var handler = CreateHandler(userRepository, resetTokens, new FakeRefreshTokenRepository(), now);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new ResetPasswordCommand(user.Email, "999999", "new-password-123"), CancellationToken.None));

        Assert.Equal(1, token.FailedAttempts);
        Assert.Null(token.UsedAt);
    }

    [Fact]
    public async Task Handle_TooManyFailedAttempts_BurnsTokenAndThrows()
    {
        var now = DateTimeOffset.UtcNow;
        var user = ExistingUser();
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(user);

        var resetTokens = new FakePasswordResetTokenRepository();
        var token = PendingToken(user.Id, now);
        token.FailedAttempts = 5;
        resetTokens.Add(token);

        var handler = CreateHandler(userRepository, resetTokens, new FakeRefreshTokenRepository(), now);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new ResetPasswordCommand(user.Email, ValidCode, "new-password-123"), CancellationToken.None));

        Assert.NotNull(token.UsedAt);
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsAuthenticationFailedException()
    {
        var now = DateTimeOffset.UtcNow;
        var handler = CreateHandler(
            new FakeUserRepository(), new FakePasswordResetTokenRepository(), new FakeRefreshTokenRepository(), now);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new ResetPasswordCommand("nobody@example.com", ValidCode, "new-password-123"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NoPendingToken_ThrowsAuthenticationFailedException()
    {
        var now = DateTimeOffset.UtcNow;
        var user = ExistingUser();
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(user);

        var handler = CreateHandler(
            userRepository, new FakePasswordResetTokenRepository(), new FakeRefreshTokenRepository(), now);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new ResetPasswordCommand(user.Email, ValidCode, "new-password-123"), CancellationToken.None));
    }
}
