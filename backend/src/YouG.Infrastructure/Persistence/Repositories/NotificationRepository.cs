using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class NotificationRepository(YouGDbContext dbContext) : INotificationRepository
{
    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public Task<List<Notification>> GetForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Notifications.Where(n => n.UserId == userId).ToListAsync(cancellationToken);

    public Task<List<Notification>> GetUnreadForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(cancellationToken);

    public void Add(Notification notification) => dbContext.Notifications.Add(notification);
}
