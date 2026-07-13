using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Common.Interfaces;

public interface IFriendRequestRepository
{
    Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Looks up a relationship row in either direction — a pair is unordered from the domain's perspective.</summary>
    Task<FriendRequest?> GetBetweenAsync(Guid userIdA, Guid userIdB, CancellationToken cancellationToken);

    Task<List<FriendRequest>> GetAcceptedForUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<FriendRequest>> GetByStatusForUserAsync(
        Guid userId, FriendRequestStatus status, CancellationToken cancellationToken);

    void Add(FriendRequest friendRequest);
    void Remove(FriendRequest friendRequest);
}
