using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class AvailabilityRuleRepository(YouGDbContext dbContext) : IAvailabilityRuleRepository
{
    public Task<AvailabilityRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.AvailabilityRules.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<List<AvailabilityRule>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.AvailabilityRules.Where(r => r.UserId == userId).ToListAsync(cancellationToken);

    public Task<List<AvailabilityRule>> GetAllAsync(CancellationToken cancellationToken) =>
        dbContext.AvailabilityRules.ToListAsync(cancellationToken);

    public void Add(AvailabilityRule rule) => dbContext.AvailabilityRules.Add(rule);

    public void Remove(AvailabilityRule rule) => dbContext.AvailabilityRules.Remove(rule);
}
