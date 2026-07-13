using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakeAvailabilityRuleRepository : IAvailabilityRuleRepository
{
    public List<AvailabilityRule> Rules { get; } = [];

    public Task<AvailabilityRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Rules.FirstOrDefault(r => r.Id == id));

    public Task<List<AvailabilityRule>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Rules.Where(r => r.UserId == userId).ToList());

    public Task<List<AvailabilityRule>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Rules.ToList());

    public void Add(AvailabilityRule rule) => Rules.Add(rule);

    public void Remove(AvailabilityRule rule) => Rules.Remove(rule);
}
