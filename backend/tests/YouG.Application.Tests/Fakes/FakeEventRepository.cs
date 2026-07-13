using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeEventRepository : IEventRepository
{
    public List<Event> Events { get; } = [];

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Events.FirstOrDefault(e => e.Id == id));

    public Task<List<Event>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken) =>
        Task.FromResult(Events.Where(e => e.GroupId == groupId).ToList());

    public Task<List<Event>> GetByGroupIdsAsync(IReadOnlyCollection<Guid> groupIds, CancellationToken cancellationToken) =>
        Task.FromResult(Events.Where(e => groupIds.Contains(e.GroupId)).ToList());

    public void Add(Event @event) => Events.Add(@event);
}
