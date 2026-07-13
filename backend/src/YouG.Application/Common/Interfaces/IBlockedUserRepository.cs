using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IBlockedUserRepository
{
    Task<BlockedUser?> GetAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken);

    /// <summary>True if either user has blocked the other.</summary>
    Task<bool> ExistsBetweenAsync(Guid userIdA, Guid userIdB, CancellationToken cancellationToken);

    void Add(BlockedUser blockedUser);
    void Remove(BlockedUser blockedUser);
}
