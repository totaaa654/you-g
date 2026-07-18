using MediatR;
using YouG.Application.Features.Notifications.Dtos;

namespace YouG.Application.Features.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery : IRequest<List<NotificationDto>>;
