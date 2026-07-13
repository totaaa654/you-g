using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class EventTimeOptionRepository(YouGDbContext dbContext) : IEventTimeOptionRepository
{
    public Task<EventTimeOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.EventTimeOptions.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<List<EventTimeOption>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        dbContext.EventTimeOptions.Where(o => o.EventId == eventId).ToListAsync(cancellationToken);

    public void Add(EventTimeOption option) => dbContext.EventTimeOptions.Add(option);
}
