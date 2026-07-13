using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Friends.Queries.GetMutualFriends;

public class GetMutualFriendsQueryHandler(
    IFriendRequestRepository friendRequestRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMutualFriendsQuery, List<PublicProfileDto>>
{
    public async Task<List<PublicProfileDto>> Handle(GetMutualFriendsQuery request, CancellationToken cancellationToken)
    {
        var myFriendships = await friendRequestRepository.GetAcceptedForUserAsync(currentUser.UserId, cancellationToken);
        var theirFriendships = await friendRequestRepository.GetAcceptedForUserAsync(request.OtherUserId, cancellationToken);

        var myFriendIds = myFriendships
            .Select(f => f.RequesterId == currentUser.UserId ? f.AddresseeId : f.RequesterId)
            .ToHashSet();
        var theirFriendIds = theirFriendships
            .Select(f => f.RequesterId == request.OtherUserId ? f.AddresseeId : f.RequesterId)
            .ToHashSet();

        var mutualIds = myFriendIds.Intersect(theirFriendIds).ToList();
        var users = await userRepository.GetByIdsAsync(mutualIds, cancellationToken);

        return users.Select(u => u.ToPublicProfileDto()).ToList();
    }
}
