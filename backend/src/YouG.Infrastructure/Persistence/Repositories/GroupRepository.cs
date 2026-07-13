using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class GroupRepository(YouGDbContext dbContext) : IGroupRepository
{
    public Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public void Add(Group group) => dbContext.Groups.Add(group);
}
