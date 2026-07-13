using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Friends.Commands.BlockUser;

public class BlockUserCommandHandler(
    IBlockedUserRepository blockedUserRepository,
    IFriendRequestRepository friendRequestRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<BlockUserCommand>
{
    public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        if (request.BlockedUserId == currentUser.UserId)
        {
            throw new ConflictException("You can't block yourself.");
        }

        var target = await userRepository.GetByIdAsync(request.BlockedUserId, cancellationToken);
        if (target is null || target.IsDeleted)
        {
            throw new NotFoundException("User not found.");
        }

        var existingBlock = await blockedUserRepository.GetAsync(currentUser.UserId, request.BlockedUserId, cancellationToken);
        if (existingBlock is not null)
        {
            return;
        }

        // Blocking dissolves any existing friendship or pending request between the two users.
        var friendRequest = await friendRequestRepository.GetBetweenAsync(
            currentUser.UserId, request.BlockedUserId, cancellationToken);
        if (friendRequest is not null)
        {
            friendRequestRepository.Remove(friendRequest);
        }

        blockedUserRepository.Add(new BlockedUser
        {
            BlockerId = currentUser.UserId,
            BlockedId = request.BlockedUserId,
            CreatedAt = dateTimeProvider.UtcNow
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
