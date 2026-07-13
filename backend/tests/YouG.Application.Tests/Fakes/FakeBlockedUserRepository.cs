using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeBlockedUserRepository : IBlockedUserRepository
{
    public List<BlockedUser> BlockedUsers { get; } = [];

    public Task<BlockedUser?> GetAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken) =>
        Task.FromResult(BlockedUsers.FirstOrDefault(b => b.BlockerId == blockerId && b.BlockedId == blockedId));

    public Task<bool> ExistsBetweenAsync(Guid userIdA, Guid userIdB, CancellationToken cancellationToken) =>
        Task.FromResult(BlockedUsers.Any(b =>
            (b.BlockerId == userIdA && b.BlockedId == userIdB)
            || (b.BlockerId == userIdB && b.BlockedId == userIdA)));

    public void Add(BlockedUser blockedUser) => BlockedUsers.Add(blockedUser);

    public void Remove(BlockedUser blockedUser) => BlockedUsers.Remove(blockedUser);
}
