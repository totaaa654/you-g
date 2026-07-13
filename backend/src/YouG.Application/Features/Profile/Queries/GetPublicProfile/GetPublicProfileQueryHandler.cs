using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Queries.GetPublicProfile;

public class GetPublicProfileQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetPublicProfileQuery, PublicProfileDto>
{
    public async Task<PublicProfileDto> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        // A deleted account looks identical to a never-existing one from the outside.
        if (user is null || user.IsDeleted)
        {
            throw new NotFoundException("User not found.");
        }

        return new PublicProfileDto(user.Id, user.Username, user.DisplayName, user.Bio, user.ProfilePictureUrl, user.FriendCode);
    }
}
