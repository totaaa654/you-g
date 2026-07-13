using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Fakes;

public class FakeFriendRequestRepository : IFriendRequestRepository
{
    public List<FriendRequest> FriendRequests { get; } = [];

    public Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(FriendRequests.FirstOrDefault(f => f.Id == id));

    public Task<FriendRequest?> GetBetweenAsync(Guid userIdA, Guid userIdB, CancellationToken cancellationToken) =>
        Task.FromResult(FriendRequests.FirstOrDefault(f =>
            (f.RequesterId == userIdA && f.AddresseeId == userIdB)
            || (f.RequesterId == userIdB && f.AddresseeId == userIdA)));

    public Task<List<FriendRequest>> GetAcceptedForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(FriendRequests
            .Where(f => f.Status == FriendRequestStatus.Accepted && (f.RequesterId == userId || f.AddresseeId == userId))
            .ToList());

    public Task<List<FriendRequest>> GetByStatusForUserAsync(
        Guid userId, FriendRequestStatus status, CancellationToken cancellationToken) =>
        Task.FromResult(FriendRequests
            .Where(f => f.Status == status && (f.RequesterId == userId || f.AddresseeId == userId))
            .ToList());

    public void Add(FriendRequest friendRequest) => FriendRequests.Add(friendRequest);

    public void Remove(FriendRequest friendRequest) => FriendRequests.Remove(friendRequest);
}
