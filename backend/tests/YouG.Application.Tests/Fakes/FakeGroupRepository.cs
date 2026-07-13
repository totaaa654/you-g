using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeGroupRepository : IGroupRepository
{
    public List<Group> Groups { get; } = [];

    public Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Groups.FirstOrDefault(g => g.Id == id));

    public void Add(Group group) => Groups.Add(group);
}
