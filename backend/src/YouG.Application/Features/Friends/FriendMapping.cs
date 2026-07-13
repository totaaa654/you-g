using YouG.Application.Features.Profile.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Friends;

internal static class FriendMapping
{
    public static PublicProfileDto ToPublicProfileDto(this User user) =>
        new(user.Id, user.Username, user.DisplayName, user.Bio, user.ProfilePictureUrl, user.FriendCode);
}
