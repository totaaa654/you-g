using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Commands.SetFavorite;

public class SetFavoriteCommandHandler(
    IFriendRequestRepository friendRequestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<SetFavoriteCommand>
{
    public async Task Handle(SetFavoriteCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await friendRequestRepository.GetBetweenAsync(
            currentUser.UserId, request.FriendUserId, cancellationToken);

        if (friendRequest is null || friendRequest.Status != FriendRequestStatus.Accepted)
        {
            throw new NotFoundException("Friendship not found.");
        }

        if (friendRequest.RequesterId == currentUser.UserId)
        {
            friendRequest.RequesterFavorited = request.IsFavorite;
        }
        else
        {
            friendRequest.AddresseeFavorited = request.IsFavorite;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
