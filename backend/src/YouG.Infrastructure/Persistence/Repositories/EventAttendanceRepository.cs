using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class EventAttendanceRepository(YouGDbContext dbContext) : IEventAttendanceRepository
{
    public Task<EventAttendance?> GetAsync(Guid eventId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.EventAttendances.FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId, cancellationToken);

    public Task<List<EventAttendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        dbContext.EventAttendances.Where(a => a.EventId == eventId).ToListAsync(cancellationToken);

    public void Add(EventAttendance attendance) => dbContext.EventAttendances.Add(attendance);
}
