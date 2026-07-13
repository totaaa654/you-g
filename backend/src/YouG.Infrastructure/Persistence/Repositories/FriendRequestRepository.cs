using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Infrastructure.Persistence.Repositories;

public class FriendRequestRepository(YouGDbContext dbContext) : IFriendRequestRepository
{
    public Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.FriendRequests.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public Task<FriendRequest?> GetBetweenAsync(Guid userIdA, Guid userIdB, CancellationToken cancellationToken) =>
        dbContext.FriendRequests.FirstOrDefaultAsync(
            f => (f.RequesterId == userIdA && f.AddresseeId == userIdB)
              || (f.RequesterId == userIdB && f.AddresseeId == userIdA),
            cancellationToken);

    public Task<List<FriendRequest>> GetAcceptedForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.FriendRequests
            .Where(f => f.Status == FriendRequestStatus.Accepted && (f.RequesterId == userId || f.AddresseeId == userId))
            .ToListAsync(cancellationToken);

    public Task<List<FriendRequest>> GetByStatusForUserAsync(
        Guid userId, FriendRequestStatus status, CancellationToken cancellationToken) =>
        dbContext.FriendRequests
            .Where(f => f.Status == status && (f.RequesterId == userId || f.AddresseeId == userId))
            .ToListAsync(cancellationToken);

    public void Add(FriendRequest friendRequest) => dbContext.FriendRequests.Add(friendRequest);

    public void Remove(FriendRequest friendRequest) => dbContext.FriendRequests.Remove(friendRequest);
}
