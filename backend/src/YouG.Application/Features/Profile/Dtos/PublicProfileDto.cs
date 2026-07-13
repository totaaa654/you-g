namespace YouG.Application.Features.Profile.Dtos;

/// <summary>The safe subset shown for any other user — no email, matches the friend-add/search flow.</summary>
public record PublicProfileDto(
    Guid Id,
    string Username,
    string DisplayName,
    string? Bio,
    string? ProfilePictureUrl,
    string FriendCode);
