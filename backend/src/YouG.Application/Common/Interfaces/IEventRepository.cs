using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Event>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    void Add(Event @event);
}
