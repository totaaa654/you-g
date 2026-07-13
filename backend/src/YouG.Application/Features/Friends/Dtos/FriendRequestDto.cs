using YouG.Application.Features.Profile.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Dtos;

public record FriendRequestDto(Guid Id, PublicProfileDto Profile, FriendRequestStatus Status, DateTimeOffset CreatedAt);
