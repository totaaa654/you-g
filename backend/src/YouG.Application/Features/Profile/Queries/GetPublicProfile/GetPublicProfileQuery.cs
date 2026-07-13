using MediatR;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Queries.GetPublicProfile;

public record GetPublicProfileQuery(Guid UserId) : IRequest<PublicProfileDto>;
