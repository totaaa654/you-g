using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Dtos;

public record GroupMemberDto(
    Guid UserId,
    string Username,
    string DisplayName,
    string? ProfilePictureUrl,
    GroupRole Role,
    DateTimeOffset JoinedAt);
