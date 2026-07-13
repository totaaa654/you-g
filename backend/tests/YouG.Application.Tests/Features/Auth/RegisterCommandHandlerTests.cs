using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Auth.Commands.Register;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Auth;

public class RegisterCommandHandlerTests
{
    private static RegisterCommandHandler CreateHandler(
        FakeUserRepository userRepository,
        FakeRefreshTokenRepository? refreshTokenRepository = null) =>
        new(
            userRepository,
            refreshTokenRepository ?? new FakeRefreshTokenRepository(),
            new FakeUnitOfWork(),
            new FakePasswordHasher(),
            new FakeTokenService(),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow));

    [Fact]
    public async Task Handle_WithNewEmailAndUsername_CreatesUserAndReturnsTokens()
    {
        var userRepository = new FakeUserRepository();
        var refreshTokenRepository = new FakeRefreshTokenRepository();
        var handler = CreateHandler(userRepository, refreshTokenRepository);

        var command = new RegisterCommand("maya@example.com", "correct-horse-battery", "maya22", "Maya", "Australia/Sydney");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("maya22", result.User.Username);
        Assert.StartsWith("YG-", result.User.FriendCode);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Single(userRepository.Users);
        Assert.Single(refreshTokenRepository.Tokens);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsConflictException()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(new User
        {
            Email = "maya@example.com",
            Username = "existing",
            FriendCode = "YG-000000",
            DisplayName = "Existing",
            TimeZoneId = "UTC"
        });

        var handler = CreateHandler(userRepository);
        var command = new RegisterCommand("maya@example.com", "correct-horse-battery", "newuser", "New", "UTC");

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateUsername_ThrowsConflictException()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(new User
        {
            Email = "other@example.com",
            Username = "maya22",
            FriendCode = "YG-000000",
            DisplayName = "Existing",
            TimeZoneId = "UTC"
        });

        var handler = CreateHandler(userRepository);
        var command = new RegisterCommand("new@example.com", "correct-horse-battery", "maya22", "New", "UTC");

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
    }
}
