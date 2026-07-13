using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IGroupInviteLinkRepository
{
    Task<GroupInviteLink?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken);
    void Add(GroupInviteLink inviteLink);
}
