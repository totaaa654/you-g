using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Common.Interfaces;

public interface IGroupJoinRequestRepository
{
    Task<GroupJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<GroupJoinRequest?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
    Task<List<GroupJoinRequest>> GetByStatusForGroupAsync(
        Guid groupId, GroupJoinRequestStatus status, CancellationToken cancellationToken);
    void Add(GroupJoinRequest joinRequest);
}
