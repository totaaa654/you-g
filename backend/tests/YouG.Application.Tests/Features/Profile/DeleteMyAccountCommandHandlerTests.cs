using YouG.Application.Features.Profile.Commands.DeleteMyAccount;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Profile;

public class DeleteMyAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ScrubsPii_AndMarksDeleted()
    {
        var users = new FakeUserRepository();
        var user = new User
        {
            Email = "maya@example.com",
            PasswordHash = "hashed:password",
            Username = "mayag",
            FriendCode = "YG-ABC123",
            DisplayName = "Maya",
            Bio = "Loves board games",
            ProfilePictureUrl = "https://example.com/pic.jpg",
            TimeZoneId = "Australia/Sydney",
            GoogleId = "google-123"
        };
        users.Users.Add(user);

        var tokens = new FakeRefreshTokenRepository();
        var now = DateTimeOffset.UtcNow;

        var handler = new DeleteMyAccountCommandHandler(
            users, tokens, new FakeUnitOfWork(), new FakeCurrentUserService(user.Id), new FakeDateTimeProvider(now));

        await handler.Handle(new DeleteMyAccountCommand(), CancellationToken.None);

        Assert.True(user.IsDeleted);
        Assert.Equal(now, user.DeletedAt);
        Assert.Null(user.Bio);
        Assert.Null(user.ProfilePictureUrl);
        Assert.Null(user.PasswordHash);
        Assert.Null(user.GoogleId);
        Assert.Equal("Deleted User", user.DisplayName);

        // Scrubbed values must still satisfy the DB's NOT NULL + unique constraints.
        Assert.NotEqual("maya@example.com", user.Email);
        Assert.NotEqual("mayag", user.Username);
        Assert.NotEqual("YG-ABC123", user.FriendCode);
        Assert.True(user.FriendCode.Length <= 10);
        Assert.NotNull(user.Email);
        Assert.NotNull(user.Username);
        Assert.NotNull(user.FriendCode);
    }

    [Fact]
    public async Task Handle_RevokesAllActiveRefreshTokens()
    {
        var users = new FakeUserRepository();
        var user = new User
        {
            Email = "maya@example.com",
            Username = "mayag",
            FriendCode = "YG-ABC123",
            DisplayName = "Maya",
            TimeZoneId = "UTC"
        };
        users.Users.Add(user);

        var tokens = new FakeRefreshTokenRepository();
        var activeToken = new RefreshToken { UserId = user.Id, TokenHash = "hash-1", ExpiresAt = DateTimeOffset.UtcNow.AddDays(10) };
        var alreadyRevokedToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = "hash-2",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(10),
            RevokedAt = DateTimeOffset.UtcNow.AddDays(-1),
        };
        tokens.Tokens.Add(activeToken);
        tokens.Tokens.Add(alreadyRevokedToken);

        var now = DateTimeOffset.UtcNow;
        var handler = new DeleteMyAccountCommandHandler(
            users, tokens, new FakeUnitOfWork(), new FakeCurrentUserService(user.Id), new FakeDateTimeProvider(now));

        await handler.Handle(new DeleteMyAccountCommand(), CancellationToken.None);

        Assert.Equal(now, activeToken.RevokedAt);
        Assert.NotEqual(now, alreadyRevokedToken.RevokedAt); // untouched, was already revoked earlier
    }
}
