using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeGroupMemberRepository(FakeGroupRepository groupRepository) : IGroupMemberRepository
{
    public List<GroupMember> Members { get; } = [];

    public Task<GroupMember?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Members.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId));

    public Task<List<GroupMember>> GetMembersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken) =>
        Task.FromResult(Members.Where(m => m.GroupId == groupId).ToList());

    public Task<int> CountMembersAsync(Guid groupId, CancellationToken cancellationToken) =>
        Task.FromResult(Members.Count(m => m.GroupId == groupId));

    public Task<List<Group>> GetGroupsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var groupIds = Members.Where(m => m.UserId == userId).Select(m => m.GroupId).ToHashSet();
        return Task.FromResult(groupRepository.Groups.Where(g => groupIds.Contains(g.Id)).ToList());
    }

    public void Add(GroupMember member) => Members.Add(member);

    public void Remove(GroupMember member) => Members.Remove(member);
}
