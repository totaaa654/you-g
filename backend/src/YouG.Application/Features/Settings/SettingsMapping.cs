using YouG.Application.Features.Settings.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Settings;

internal static class SettingsMapping
{
    public static SettingsDto ToSettingsDto(this User user) => new(
        user.ThemePreference, user.IsSearchable, user.NotifyOnFriendRequest,
        user.NotifyOnGroupInvite, user.NotifyOnEventReminder, user.NotifyOnScheduleUpdate);
}
