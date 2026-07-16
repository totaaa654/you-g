using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Groups.Dtos;

public record GroupJoinRequestDto(Guid Id, PublicProfileDto Profile, DateTimeOffset CreatedAt);
