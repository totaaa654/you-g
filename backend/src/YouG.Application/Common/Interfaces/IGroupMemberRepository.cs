using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IGroupMemberRepository
{
    Task<GroupMember?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
    Task<List<GroupMember>> GetMembersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    Task<int> CountMembersAsync(Guid groupId, CancellationToken cancellationToken);
    Task<List<Group>> GetGroupsForUserAsync(Guid userId, CancellationToken cancellationToken);
    void Add(GroupMember member);
    void Remove(GroupMember member);
}
