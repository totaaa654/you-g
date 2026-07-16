using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Infrastructure.Persistence.Repositories;

public class GroupJoinRequestRepository(YouGDbContext dbContext) : IGroupJoinRequestRepository
{
    public Task<GroupJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.GroupJoinRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<GroupJoinRequest?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.GroupJoinRequests.FirstOrDefaultAsync(r => r.GroupId == groupId && r.UserId == userId, cancellationToken);

    public Task<List<GroupJoinRequest>> GetByStatusForGroupAsync(
        Guid groupId, GroupJoinRequestStatus status, CancellationToken cancellationToken) =>
        dbContext.GroupJoinRequests
            .Where(r => r.GroupId == groupId && r.Status == status)
            .ToListAsync(cancellationToken);

    public void Add(GroupJoinRequest joinRequest) => dbContext.GroupJoinRequests.Add(joinRequest);
}
