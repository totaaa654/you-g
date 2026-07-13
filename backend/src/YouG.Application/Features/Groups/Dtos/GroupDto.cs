namespace YouG.Application.Features.Groups.Dtos;

public record GroupDto(
    Guid Id,
    string Name,
    string? Description,
    string? PictureUrl,
    Guid CreatedByUserId,
    int MemberCount,
    DateTimeOffset CreatedAt);
