using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Friends.Dtos;

namespace YouG.Application.Features.Friends.Queries.GetMyFriends;

public class GetMyFriendsQueryHandler(
    IFriendRequestRepository friendRequestRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyFriendsQuery, List<FriendDto>>
{
    public async Task<List<FriendDto>> Handle(GetMyFriendsQuery request, CancellationToken cancellationToken)
    {
        var friendships = await friendRequestRepository.GetAcceptedForUserAsync(currentUser.UserId, cancellationToken);

        var otherUserIds = friendships
            .Select(f => f.RequesterId == currentUser.UserId ? f.AddresseeId : f.RequesterId)
            .ToList();
        var usersById = (await userRepository.GetByIdsAsync(otherUserIds, cancellationToken))
            .ToDictionary(u => u.Id);

        return friendships
            .Select(f =>
            {
                var isCallerRequester = f.RequesterId == currentUser.UserId;
                var otherUserId = isCallerRequester ? f.AddresseeId : f.RequesterId;
                var isFavorite = isCallerRequester ? f.RequesterFavorited : f.AddresseeFavorited;
                return (Friendship: f, OtherUserId: otherUserId, IsFavorite: isFavorite);
            })
            .Where(x => usersById.ContainsKey(x.OtherUserId))
            .Select(x => new FriendDto(
                usersById[x.OtherUserId].ToPublicProfileDto(),
                x.IsFavorite,
                x.Friendship.RespondedAt ?? x.Friendship.CreatedAt))
            .ToList();
    }
}
