using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class BlockedUserRepository(YouGDbContext dbContext) : IBlockedUserRepository
{
    public Task<BlockedUser?> GetAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken) =>
        dbContext.BlockedUsers.FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId, cancellationToken);

    public Task<bool> ExistsBetweenAsync(Guid userIdA, Guid userIdB, CancellationToken cancellationToken) =>
        dbContext.BlockedUsers.AnyAsync(
            b => (b.BlockerId == userIdA && b.BlockedId == userIdB)
              || (b.BlockerId == userIdB && b.BlockedId == userIdA),
            cancellationToken);

    public void Add(BlockedUser blockedUser) => dbContext.BlockedUsers.Add(blockedUser);

    public void Remove(BlockedUser blockedUser) => dbContext.BlockedUsers.Remove(blockedUser);
}
