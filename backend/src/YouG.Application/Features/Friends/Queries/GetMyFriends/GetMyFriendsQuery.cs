using MediatR;
using YouG.Application.Features.Friends.Dtos;

namespace YouG.Application.Features.Friends.Queries.GetMyFriends;

public record GetMyFriendsQuery : IRequest<List<FriendDto>>;
