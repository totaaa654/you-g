using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Group group);
}
