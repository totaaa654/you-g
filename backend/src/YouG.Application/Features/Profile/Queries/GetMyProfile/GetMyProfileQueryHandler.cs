using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyProfileQuery, ProfileDto>
{
    public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return new ProfileDto(
            user.Id, user.Email, user.Username, user.DisplayName, user.Bio,
            user.ProfilePictureUrl, user.TimeZoneId, user.FriendCode, user.CreatedAt);
    }
}
