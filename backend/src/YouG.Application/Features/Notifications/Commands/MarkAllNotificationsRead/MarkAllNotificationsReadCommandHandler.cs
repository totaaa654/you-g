using MediatR;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<MarkAllNotificationsReadCommand>
{
    public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var unread = await notificationRepository.GetUnreadForUserAsync(currentUser.UserId, cancellationToken);

        if (unread.Count == 0)
        {
            return;
        }

        foreach (var notification in unread)
        {
            notification.IsRead = true;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
