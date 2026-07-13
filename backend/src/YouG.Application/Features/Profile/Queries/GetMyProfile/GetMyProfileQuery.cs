using MediatR;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<ProfileDto>;
