using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IEventAttendanceRepository
{
    Task<EventAttendance?> GetAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task<List<EventAttendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    void Add(EventAttendance attendance);
}
