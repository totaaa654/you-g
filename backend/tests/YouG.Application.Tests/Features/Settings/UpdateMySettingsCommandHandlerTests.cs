using YouG.Application.Features.Settings.Commands.UpdateMySettings;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Settings;

public class UpdateMySettingsCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesAllSettingsFields()
    {
        var user = new User
        {
            Email = "maya@example.com", Username = "mayag", FriendCode = "YG-MAYA01",
            DisplayName = "Maya", TimeZoneId = "UTC"
        };
        var users = new FakeUserRepository();
        users.Add(user);

        var handler = new UpdateMySettingsCommandHandler(
            users, new FakeUnitOfWork(), new FakeCurrentUserService(user.Id), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.Handle(
            new UpdateMySettingsCommand(ThemeMode.Dark, false, false, true, false, true), CancellationToken.None);

        Assert.Equal(ThemeMode.Dark, result.ThemePreference);
        Assert.False(result.IsSearchable);
        Assert.False(result.NotifyOnFriendRequest);
        Assert.True(result.NotifyOnGroupInvite);
        Assert.False(result.NotifyOnEventReminder);
        Assert.True(result.NotifyOnScheduleUpdate);

        Assert.Equal(ThemeMode.Dark, user.ThemePreference);
        Assert.False(user.IsSearchable);
    }
}
