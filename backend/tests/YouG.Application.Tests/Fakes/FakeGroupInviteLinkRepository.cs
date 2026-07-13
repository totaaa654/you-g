using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeGroupInviteLinkRepository : IGroupInviteLinkRepository
{
    public List<GroupInviteLink> Links { get; } = [];

    public Task<GroupInviteLink?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        Task.FromResult(Links.FirstOrDefault(l => l.Code == code));

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken) =>
        Task.FromResult(Links.Any(l => l.Code == code));

    public void Add(GroupInviteLink inviteLink) => Links.Add(inviteLink);
}
