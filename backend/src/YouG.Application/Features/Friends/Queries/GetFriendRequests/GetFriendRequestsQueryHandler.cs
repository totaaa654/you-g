using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Friends.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler(
    IFriendRequestRepository friendRequestRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetFriendRequestsQuery, List<FriendRequestDto>>
{
    public async Task<List<FriendRequestDto>> Handle(GetFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        var pending = await friendRequestRepository.GetByStatusForUserAsync(
            currentUser.UserId, FriendRequestStatus.Pending, cancellationToken);

        var relevant = request.Direction == FriendRequestDirection.Incoming
            ? pending.Where(f => f.AddresseeId == currentUser.UserId).ToList()
            : pending.Where(f => f.RequesterId == currentUser.UserId).ToList();

        var otherUserIds = relevant
            .Select(f => request.Direction == FriendRequestDirection.Incoming ? f.RequesterId : f.AddresseeId)
            .ToList();
        var usersById = (await userRepository.GetByIdsAsync(otherUserIds, cancellationToken))
            .ToDictionary(u => u.Id);

        return relevant
            .Select(f => (Request: f, OtherUserId: request.Direction == FriendRequestDirection.Incoming ? f.RequesterId : f.AddresseeId))
            .Where(x => usersById.ContainsKey(x.OtherUserId))
            .Select(x => new FriendRequestDto(
                x.Request.Id, usersById[x.OtherUserId].ToPublicProfileDto(), x.Request.Status, x.Request.CreatedAt))
            .ToList();
    }
}
