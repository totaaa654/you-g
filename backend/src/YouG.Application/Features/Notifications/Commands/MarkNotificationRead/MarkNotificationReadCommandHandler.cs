using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Notifications.Commands.MarkNotificationRead;

public class MarkNotificationReadCommandHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<MarkNotificationReadCommand>
{
    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null || notification.UserId != currentUser.UserId)
        {
            throw new NotFoundException("Notification not found.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
