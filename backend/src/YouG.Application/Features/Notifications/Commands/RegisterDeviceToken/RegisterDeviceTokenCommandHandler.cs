using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Notifications.Commands.RegisterDeviceToken;

public class RegisterDeviceTokenCommandHandler(
    IDeviceTokenRepository deviceTokenRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<RegisterDeviceTokenCommand>
{
    public async Task Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var existing = await deviceTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (existing is null)
        {
            deviceTokenRepository.Add(new DeviceToken
            {
                UserId = currentUser.UserId,
                Token = request.Token,
                Platform = request.Platform,
                LastUsedAt = now,
                CreatedAt = now
            });
        }
        else
        {
            // Same physical device token can outlive a logout/login-as-someone-else cycle —
            // re-point it at whoever is currently authenticated on this device instead of duplicating.
            existing.UserId = currentUser.UserId;
            existing.Platform = request.Platform;
            existing.LastUsedAt = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
