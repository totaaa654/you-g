using MediatR;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Friends.Queries.GetMutualFriends;

public record GetMutualFriendsQuery(Guid OtherUserId) : IRequest<List<PublicProfileDto>>;
