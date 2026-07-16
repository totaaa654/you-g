using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IAvailabilityInstanceRepository
{
    Task<AvailabilityInstance?> GetAsync(Guid userId, DateOnly date, TimeOnly startTime, CancellationToken cancellationToken);

    Task<List<AvailabilityInstance>> GetForUserInRangeAsync(
        Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    /// <summary>For the Smart Time Finder / heatmap — every instance for every member of a group in a date range.</summary>
    Task<List<AvailabilityInstance>> GetForUsersInRangeAsync(
        IReadOnlyCollection<Guid> userIds, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    void Add(AvailabilityInstance instance);
}
