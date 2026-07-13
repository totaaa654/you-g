using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class GroupInviteLinkRepository(YouGDbContext dbContext) : IGroupInviteLinkRepository
{
    public Task<GroupInviteLink?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        dbContext.GroupInviteLinks.FirstOrDefaultAsync(l => l.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken) =>
        dbContext.GroupInviteLinks.AnyAsync(l => l.Code == code, cancellationToken);

    public void Add(GroupInviteLink inviteLink) => dbContext.GroupInviteLinks.Add(inviteLink);
}
