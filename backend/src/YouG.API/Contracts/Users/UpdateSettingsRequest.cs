using YouG.Domain.Enums;

namespace YouG.API.Contracts.Users;

public record UpdateSettingsRequest(
    ThemeMode ThemePreference,
    bool IsSearchable,
    bool NotifyOnFriendRequest,
    bool NotifyOnGroupInvite,
    bool NotifyOnEventReminder,
    bool NotifyOnScheduleUpdate);
