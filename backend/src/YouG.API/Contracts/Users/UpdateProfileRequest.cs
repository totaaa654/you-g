namespace YouG.API.Contracts.Users;

public record UpdateProfileRequest(string DisplayName, string? Bio, string TimeZoneId, string? ProfilePictureUrl);
