using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Notifications.Dtos;

namespace YouG.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler(
    INotificationRepository notificationRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
{
    public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetForUserAsync(currentUser.UserId, cancellationToken);
        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => n.ToDto())
            .ToList();
    }
}
