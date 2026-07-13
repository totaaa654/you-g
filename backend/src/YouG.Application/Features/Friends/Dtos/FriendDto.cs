using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Friends.Dtos;

public record FriendDto(PublicProfileDto Profile, bool IsFavorite, DateTimeOffset FriendedSince);
