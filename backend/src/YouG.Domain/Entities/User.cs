using YouG.Domain.Common;
using YouG.Domain.Enums;

namespace YouG.Domain.Entities;

public class User : Entity
{
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? GoogleId { get; set; }
    public required string Username { get; set; }
    public required string FriendCode { get; set; }
    public required string DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public required string TimeZoneId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ThemeMode ThemePreference { get; set; } = ThemeMode.System;

    /// <summary>Off = excluded from username search; friends/group co-members still see the profile normally.</summary>
    public bool IsSearchable { get; set; } = true;

    public bool NotifyOnFriendRequest { get; set; } = true;
    public bool NotifyOnGroupInvite { get; set; } = true;
    public bool NotifyOnEventReminder { get; set; } = true;
    public bool NotifyOnScheduleUpdate { get; set; } = true;
}
