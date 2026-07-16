using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Fakes;

public class FakeGroupJoinRequestRepository : IGroupJoinRequestRepository
{
    public List<GroupJoinRequest> Requests { get; } = [];

    public Task<GroupJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Requests.FirstOrDefault(r => r.Id == id));

    public Task<GroupJoinRequest?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Requests.FirstOrDefault(r => r.GroupId == groupId && r.UserId == userId));

    public Task<List<GroupJoinRequest>> GetByStatusForGroupAsync(
        Guid groupId, GroupJoinRequestStatus status, CancellationToken cancellationToken) =>
        Task.FromResult(Requests.Where(r => r.GroupId == groupId && r.Status == status).ToList());

    public void Add(GroupJoinRequest joinRequest) => Requests.Add(joinRequest);
}
