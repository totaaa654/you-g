using System.Text.Json;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Infrastructure.Push;

public class NotificationDispatcher(
    INotificationRepository notificationRepository,
    IDeviceTokenRepository deviceTokenRepository,
    IPushNotificationSender pushNotificationSender,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : INotificationDispatcher
{
    public async Task DispatchAsync(
        Guid userId, NotificationType type, string title, string body,
        IReadOnlyDictionary<string, string> payload, CancellationToken cancellationToken)
    {
        notificationRepository.Add(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Body = body,
            Payload = JsonSerializer.Serialize(payload),
            IsRead = false,
            CreatedAt = dateTimeProvider.UtcNow
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var devices = await deviceTokenRepository.GetByUserIdAsync(userId, cancellationToken);
        if (devices.Count == 0)
        {
            return;
        }

        var deadTokens = await pushNotificationSender.SendAsync(
            devices.Select(d => d.Token).ToList(), title, body, payload, cancellationToken);

        if (deadTokens.Count > 0)
        {
            var deadSet = deadTokens.ToHashSet();
            deviceTokenRepository.RemoveRange(devices.Where(d => deadSet.Contains(d.Token)));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
