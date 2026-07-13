using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class EventRepository(YouGDbContext dbContext) : IEventRepository
{
    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<List<Event>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken) =>
        dbContext.Events.Where(e => e.GroupId == groupId).ToListAsync(cancellationToken);

    public void Add(Event @event) => dbContext.Events.Add(@event);
}
