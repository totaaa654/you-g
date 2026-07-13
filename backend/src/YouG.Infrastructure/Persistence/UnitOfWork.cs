using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure.Persistence;

public class UnitOfWork(YouGDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
