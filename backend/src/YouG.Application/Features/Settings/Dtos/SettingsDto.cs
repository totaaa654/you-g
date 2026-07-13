using YouG.Domain.Enums;

namespace YouG.Application.Features.Settings.Dtos;

public record SettingsDto(
    ThemeMode ThemePreference,
    bool IsSearchable,
    bool NotifyOnFriendRequest,
    bool NotifyOnGroupInvite,
    bool NotifyOnEventReminder,
    bool NotifyOnScheduleUpdate);
