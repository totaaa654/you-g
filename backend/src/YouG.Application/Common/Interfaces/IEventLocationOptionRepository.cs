using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IEventLocationOptionRepository
{
    Task<EventLocationOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<EventLocationOption>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    void Add(EventLocationOption option);
}
