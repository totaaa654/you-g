using MediatR;
using YouG.Application.Features.Friends.Dtos;

namespace YouG.Application.Features.Friends.Queries.GetFriendRequests;

public record GetFriendRequestsQuery(FriendRequestDirection Direction) : IRequest<List<FriendRequestDto>>;
