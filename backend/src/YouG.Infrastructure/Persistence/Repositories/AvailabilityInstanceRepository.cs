using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class AvailabilityInstanceRepository(YouGDbContext dbContext) : IAvailabilityInstanceRepository
{
    public Task<AvailabilityInstance?> GetAsync(Guid userId, DateOnly date, TimeOnly startTime, CancellationToken cancellationToken) =>
        dbContext.AvailabilityInstances
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Date == date && i.StartTime == startTime, cancellationToken);

    public Task<List<AvailabilityInstance>> GetForUserInRangeAsync(
        Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken) =>
        dbContext.AvailabilityInstances
            .Where(i => i.UserId == userId && i.Date >= from && i.Date <= to)
            .ToListAsync(cancellationToken);

    public Task<List<AvailabilityInstance>> GetForUsersInRangeAsync(
        IReadOnlyCollection<Guid> userIds, DateOnly from, DateOnly to, CancellationToken cancellationToken) =>
        dbContext.AvailabilityInstances
            .Where(i => userIds.Contains(i.UserId) && i.Date >= from && i.Date <= to)
            .ToListAsync(cancellationToken);

    public void Add(AvailabilityInstance instance) => dbContext.AvailabilityInstances.Add(instance);
}
