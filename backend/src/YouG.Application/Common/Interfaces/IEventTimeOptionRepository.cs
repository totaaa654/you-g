using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IEventTimeOptionRepository
{
    Task<EventTimeOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<EventTimeOption>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    void Add(EventTimeOption option);
}
