using YouG.Domain.Enums;

namespace YouG.Application.Common.Interfaces;

/// <summary>
/// Single entry point feature handlers use to notify a user — persists a <see cref="Domain.Entities.Notification"/>
/// row (read by the Notifications screen) and best-effort pushes it to their registered devices via
/// <see cref="IPushNotificationSender"/>. A push failure never fails the calling command; the in-app
/// notification row is the source of truth.
/// </summary>
public interface INotificationDispatcher
{
    Task DispatchAsync(
        Guid userId, NotificationType type, string title, string body,
        IReadOnlyDictionary<string, string> payload, CancellationToken cancellationToken);
}
