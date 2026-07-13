using MediatR;

namespace YouG.Application.Features.Friends.Commands.RemoveFriend;

public record RemoveFriendCommand(Guid FriendUserId) : IRequest;
