namespace YouG.Application.Features.Auth.Dtos;

public record AuthResultDto(string AccessToken, string RefreshToken, UserSummaryDto User);

public record UserSummaryDto(Guid Id, string Username, string DisplayName, string FriendCode);
