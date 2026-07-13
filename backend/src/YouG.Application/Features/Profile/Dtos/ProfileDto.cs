namespace YouG.Application.Features.Profile.Dtos;

/// <summary>The full profile — only ever returned for the authenticated user themselves.</summary>
public record ProfileDto(
    Guid Id,
    string Email,
    string Username,
    string DisplayName,
    string? Bio,
    string? ProfilePictureUrl,
    string TimeZoneId,
    string FriendCode,
    DateTimeOffset CreatedAt);
