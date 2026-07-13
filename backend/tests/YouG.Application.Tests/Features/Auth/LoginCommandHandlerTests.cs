using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Auth.Commands.Login;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private static LoginCommandHandler CreateHandler(FakeUserRepository userRepository) =>
        new(
            userRepository,
            new FakeRefreshTokenRepository(),
            new FakeUnitOfWork(),
            new FakePasswordHasher(),
            new FakeTokenService(),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow));

    private static User ExistingUser(string password) => new()
    {
        Email = "maya@example.com",
        PasswordHash = $"hashed:{password}",
        Username = "maya22",
        FriendCode = "YG-000000",
        DisplayName = "Maya",
        TimeZoneId = "Australia/Sydney"
    };

    [Fact]
    public async Task Handle_WithCorrectCredentials_ReturnsTokens()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(ExistingUser("correct-horse-battery"));
        var handler = CreateHandler(userRepository);

        var result = await handler.Handle(new LoginCommand("maya@example.com", "correct-horse-battery"), CancellationToken.None);

        Assert.Equal("maya22", result.User.Username);
        Assert.NotEmpty(result.AccessToken);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsAuthenticationFailedException()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(ExistingUser("correct-horse-battery"));
        var handler = CreateHandler(userRepository);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new LoginCommand("maya@example.com", "wrong-password"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ThrowsAuthenticationFailedException()
    {
        var userRepository = new FakeUserRepository();
        var handler = CreateHandler(userRepository);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new LoginCommand("nobody@example.com", "whatever"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSoftDeletedUser_ThrowsAuthenticationFailedException()
    {
        var userRepository = new FakeUserRepository();
        var user = ExistingUser("correct-horse-battery");
        user.IsDeleted = true;
        userRepository.Users.Add(user);
        var handler = CreateHandler(userRepository);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => handler.Handle(new LoginCommand("maya@example.com", "correct-horse-battery"), CancellationToken.None));
    }
}
