using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class GroupMemberRepository(YouGDbContext dbContext) : IGroupMemberRepository
{
    public Task<GroupMember?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);

    public Task<List<GroupMember>> GetMembersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken) =>
        dbContext.GroupMembers.Where(m => m.GroupId == groupId).ToListAsync(cancellationToken);

    public Task<int> CountMembersAsync(Guid groupId, CancellationToken cancellationToken) =>
        dbContext.GroupMembers.CountAsync(m => m.GroupId == groupId, cancellationToken);

    public Task<List<Group>> GetGroupsForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.GroupMembers
            .Where(m => m.UserId == userId)
            .Join(dbContext.Groups, m => m.GroupId, g => g.Id, (m, g) => g)
            .ToListAsync(cancellationToken);

    public void Add(GroupMember member) => dbContext.GroupMembers.Add(member);

    public void Remove(GroupMember member) => dbContext.GroupMembers.Remove(member);
}
