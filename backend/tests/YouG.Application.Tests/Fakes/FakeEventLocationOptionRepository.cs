using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeEventLocationOptionRepository : IEventLocationOptionRepository
{
    public List<EventLocationOption> Options { get; } = [];

    public Task<EventLocationOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Options.FirstOrDefault(o => o.Id == id));

    public Task<List<EventLocationOption>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        Task.FromResult(Options.Where(o => o.EventId == eventId).ToList());

    public void Add(EventLocationOption option) => Options.Add(option);
}
