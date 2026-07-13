using MediatR;
using YouG.Application.Features.Friends.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Commands.RespondToFriendRequest;

public record RespondToFriendRequestCommand(Guid FriendRequestId, FriendRequestStatus Status) : IRequest<FriendRequestDto>;
