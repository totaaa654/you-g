using MediatR;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    string DisplayName,
    string? Bio,
    string TimeZoneId,
    string? ProfilePictureUrl) : IRequest<ProfileDto>;
