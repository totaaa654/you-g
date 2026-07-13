namespace YouG.API.Contracts.Auth;

public record RegisterRequest(string Email, string Password, string Username, string DisplayName, string TimeZoneId);
