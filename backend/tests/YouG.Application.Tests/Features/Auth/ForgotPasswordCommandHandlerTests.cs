using Microsoft.Extensions.Options;
using YouG.Application.Common;
using YouG.Application.Features.Auth.Commands.ForgotPassword;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Auth;

public class ForgotPasswordCommandHandlerTests
{
    private static ForgotPasswordCommandHandler CreateHandler(
        FakeUserRepository userRepository, FakePasswordResetTokenRepository resetTokens, FakeEmailSender emailSender) =>
        new(
            userRepository,
            resetTokens,
            new FakeUnitOfWork(),
            new FakeTokenService(),
            emailSender,
            Options.Create(new ClientUrlSettings()),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow));

    private static User ExistingUser() => new()
    {
        Email = "maya@example.com",
        PasswordHash = "hashed:correct-horse-battery",
        Username = "maya22",
        FriendCode = "YG-000000",
        DisplayName = "Maya",
        TimeZoneId = "Australia/Sydney"
    };

    [Fact]
    public async Task Handle_ExistingUser_CreatesTokenAndSendsEmail()
    {
        var userRepository = new FakeUserRepository();
        userRepository.Users.Add(ExistingUser());
        var resetTokens = new FakePasswordResetTokenRepository();
        var emailSender = new FakeEmailSender();
        var handler = CreateHandler(userRepository, resetTokens, emailSender);

        await handler.Handle(new ForgotPasswordCommand("maya@example.com"), CancellationToken.None);

        Assert.Single(resetTokens.Tokens);
        var sent = Assert.Single(emailSender.Sent);
        Assert.Equal("maya@example.com", sent.ToEmail);
    }

    [Fact]
    public async Task Handle_UnknownEmail_SendsNoEmailAndDoesNotThrow()
    {
        var userRepository = new FakeUserRepository();
        var resetTokens = new FakePasswordResetTokenRepository();
        var emailSender = new FakeEmailSender();
        var handler = CreateHandler(userRepository, resetTokens, emailSender);

        await handler.Handle(new ForgotPasswordCommand("nobody@example.com"), CancellationToken.None);

        Assert.Empty(resetTokens.Tokens);
        Assert.Empty(emailSender.Sent);
    }

    [Fact]
    public async Task Handle_SoftDeletedUser_SendsNoEmail()
    {
        var userRepository = new FakeUserRepository();
        var user = ExistingUser();
        user.IsDeleted = true;
        userRepository.Users.Add(user);
        var resetTokens = new FakePasswordResetTokenRepository();
        var emailSender = new FakeEmailSender();
        var handler = CreateHandler(userRepository, resetTokens, emailSender);

        await handler.Handle(new ForgotPasswordCommand("maya@example.com"), CancellationToken.None);

        Assert.Empty(emailSender.Sent);
    }

    [Fact]
    public async Task Handle_GoogleOnlyAccountWithNoPassword_SendsNoEmail()
    {
        var userRepository = new FakeUserRepository();
        var user = ExistingUser();
        user.PasswordHash = null;
        user.GoogleId = "google-123";
        userRepository.Users.Add(user);
        var resetTokens = new FakePasswordResetTokenRepository();
        var emailSender = new FakeEmailSender();
        var handler = CreateHandler(userRepository, resetTokens, emailSender);

        await handler.Handle(new ForgotPasswordCommand("maya@example.com"), CancellationToken.None);

        Assert.Empty(emailSender.Sent);
    }
}
