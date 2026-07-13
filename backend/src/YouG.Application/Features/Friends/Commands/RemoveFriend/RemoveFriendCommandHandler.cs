using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Commands.RemoveFriend;

public class RemoveFriendCommandHandler(
    IFriendRequestRepository friendRequestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<RemoveFriendCommand>
{
    public async Task Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await friendRequestRepository.GetBetweenAsync(
            currentUser.UserId, request.FriendUserId, cancellationToken);

        if (friendRequest is null || friendRequest.Status != FriendRequestStatus.Accepted)
        {
            throw new NotFoundException("Friendship not found.");
        }

        friendRequestRepository.Remove(friendRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
