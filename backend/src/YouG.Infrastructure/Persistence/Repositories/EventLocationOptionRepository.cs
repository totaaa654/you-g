using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class EventLocationOptionRepository(YouGDbContext dbContext) : IEventLocationOptionRepository
{
    public Task<EventLocationOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.EventLocationOptions.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<List<EventLocationOption>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        dbContext.EventLocationOptions.Where(o => o.EventId == eventId).ToListAsync(cancellationToken);

    public void Add(EventLocationOption option) => dbContext.EventLocationOptions.Add(option);
}
