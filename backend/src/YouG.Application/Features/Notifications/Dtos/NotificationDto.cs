using YouG.Domain.Enums;

namespace YouG.Application.Features.Notifications.Dtos;

public record NotificationDto(
    Guid Id, NotificationType Type, string Title, string Body, string Payload, bool IsRead, DateTimeOffset CreatedAt);
