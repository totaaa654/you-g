using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Profile.Dtos;
using YouG.Application.Features.Settings;

namespace YouG.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<UpdateMyProfileCommand, ProfileDto>
{
    public async Task<ProfileDto> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.DisplayName = request.DisplayName;
        user.Bio = request.Bio;
        user.TimeZoneId = request.TimeZoneId;
        user.ProfilePictureUrl = request.ProfilePictureUrl;
        user.UpdatedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProfileDto(
            user.Id, user.Email, user.Username, user.DisplayName, user.Bio,
            user.ProfilePictureUrl, user.TimeZoneId, user.FriendCode, user.CreatedAt, user.ToSettingsDto());
    }
}
