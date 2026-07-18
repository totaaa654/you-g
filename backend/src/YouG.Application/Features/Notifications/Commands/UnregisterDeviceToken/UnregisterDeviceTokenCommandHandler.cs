using MediatR;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Notifications.Commands.UnregisterDeviceToken;

public class UnregisterDeviceTokenCommandHandler(
    IDeviceTokenRepository deviceTokenRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<UnregisterDeviceTokenCommand>
{
    public async Task Handle(UnregisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await deviceTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        // Only remove a token that actually belongs to the caller — a stale/foreign token here is a no-op.
        if (existing is not null && existing.UserId == currentUser.UserId)
        {
            deviceTokenRepository.Remove(existing);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
