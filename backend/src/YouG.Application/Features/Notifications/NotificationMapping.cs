using YouG.Application.Features.Notifications.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Notifications;

internal static class NotificationMapping
{
    public static NotificationDto ToDto(this Notification notification) =>
        new(notification.Id, notification.Type, notification.Title, notification.Body, notification.Payload,
            notification.IsRead, notification.CreatedAt);
}
