using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeEventTimeOptionRepository : IEventTimeOptionRepository
{
    public List<EventTimeOption> Options { get; } = [];

    public Task<EventTimeOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Options.FirstOrDefault(o => o.Id == id));

    public Task<List<EventTimeOption>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        Task.FromResult(Options.Where(o => o.EventId == eventId).ToList());

    public void Add(EventTimeOption option) => Options.Add(option);
}
