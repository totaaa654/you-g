using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Fakes;

public class FakeAvailabilityInstanceRepository : IAvailabilityInstanceRepository
{
    public List<AvailabilityInstance> Instances { get; } = [];

    public Task<AvailabilityInstance?> GetAsync(Guid userId, DateOnly date, Daypart daypart, CancellationToken cancellationToken) =>
        Task.FromResult(Instances.FirstOrDefault(i => i.UserId == userId && i.Date == date && i.Daypart == daypart));

    public Task<List<AvailabilityInstance>> GetForUserInRangeAsync(
        Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken) =>
        Task.FromResult(Instances.Where(i => i.UserId == userId && i.Date >= from && i.Date <= to).ToList());

    public Task<List<AvailabilityInstance>> GetForUsersInRangeAsync(
        IReadOnlyCollection<Guid> userIds, DateOnly from, DateOnly to, CancellationToken cancellationToken) =>
        Task.FromResult(Instances.Where(i => userIds.Contains(i.UserId) && i.Date >= from && i.Date <= to).ToList());

    public void Add(AvailabilityInstance instance) => Instances.Add(instance);
}
