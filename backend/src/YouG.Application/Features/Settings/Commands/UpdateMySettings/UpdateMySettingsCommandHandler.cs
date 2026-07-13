using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Settings.Dtos;

namespace YouG.Application.Features.Settings.Commands.UpdateMySettings;

public class UpdateMySettingsCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpdateMySettingsCommand, SettingsDto>
{
    public async Task<SettingsDto> Handle(UpdateMySettingsCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.ThemePreference = request.ThemePreference;
        user.IsSearchable = request.IsSearchable;
        user.NotifyOnFriendRequest = request.NotifyOnFriendRequest;
        user.NotifyOnGroupInvite = request.NotifyOnGroupInvite;
        user.NotifyOnEventReminder = request.NotifyOnEventReminder;
        user.NotifyOnScheduleUpdate = request.NotifyOnScheduleUpdate;
        user.UpdatedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToSettingsDto();
    }
}
