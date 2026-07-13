using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IAvailabilityRuleRepository
{
    Task<AvailabilityRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<AvailabilityRule>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Every rule in the system — the periodic full sweep filters per-candidate-date by EffectiveFrom/EffectiveUntil itself.</summary>
    Task<List<AvailabilityRule>> GetAllAsync(CancellationToken cancellationToken);
    void Add(AvailabilityRule rule);
    void Remove(AvailabilityRule rule);
}
