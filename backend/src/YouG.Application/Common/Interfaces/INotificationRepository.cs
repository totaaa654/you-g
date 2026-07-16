using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Notification>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<Notification>> GetUnreadForUserAsync(Guid userId, CancellationToken cancellationToken);

    void Add(Notification notification);
}
