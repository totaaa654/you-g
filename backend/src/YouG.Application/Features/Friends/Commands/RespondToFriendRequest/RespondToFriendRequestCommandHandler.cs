using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Friends.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Commands.RespondToFriendRequest;

public class RespondToFriendRequestCommandHandler(
    IFriendRequestRepository friendRequestRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<RespondToFriendRequestCommand, FriendRequestDto>
{
    public async Task<FriendRequestDto> Handle(RespondToFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await friendRequestRepository.GetByIdAsync(request.FriendRequestId, cancellationToken)
            ?? throw new NotFoundException("Friend request not found.");

        FriendAuthorization.RequireParticipant(friendRequest, currentUser.UserId);

        if (friendRequest.AddresseeId != currentUser.UserId)
        {
            throw new ForbiddenException("Only the recipient can respond to a friend request.");
        }

        if (friendRequest.Status != FriendRequestStatus.Pending)
        {
            throw new ConflictException("This friend request has already been responded to.");
        }

        friendRequest.Status = request.Status;
        friendRequest.RespondedAt = dateTimeProvider.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var requester = await userRepository.GetByIdAsync(friendRequest.RequesterId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return new FriendRequestDto(friendRequest.Id, requester.ToPublicProfileDto(), friendRequest.Status, friendRequest.CreatedAt);
    }
}
