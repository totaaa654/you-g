using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeEventAttendanceRepository : IEventAttendanceRepository
{
    public List<EventAttendance> Attendance { get; } = [];

    public Task<EventAttendance?> GetAsync(Guid eventId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Attendance.FirstOrDefault(a => a.EventId == eventId && a.UserId == userId));

    public Task<List<EventAttendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken) =>
        Task.FromResult(Attendance.Where(a => a.EventId == eventId).ToList());

    public void Add(EventAttendance attendance) => Attendance.Add(attendance);
}
