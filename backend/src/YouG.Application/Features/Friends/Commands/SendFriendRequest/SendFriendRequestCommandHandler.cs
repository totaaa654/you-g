using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Friends.Dtos;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Friends.Commands.SendFriendRequest;

public class SendFriendRequestCommandHandler(
    IFriendRequestRepository friendRequestRepository,
    IBlockedUserRepository blockedUserRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<SendFriendRequestCommand, FriendRequestDto>
{
    public async Task<FriendRequestDto> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var target = request.AddresseeId.HasValue
            ? await userRepository.GetByIdAsync(request.AddresseeId.Value, cancellationToken)
            : await userRepository.GetByFriendCodeAsync(request.FriendCode!, cancellationToken);

        if (target is null || target.IsDeleted)
        {
            throw new NotFoundException("User not found.");
        }

        if (target.Id == currentUser.UserId)
        {
            throw new ConflictException("You can't send a friend request to yourself.");
        }

        if (await blockedUserRepository.ExistsBetweenAsync(currentUser.UserId, target.Id, cancellationToken))
        {
            throw new ForbiddenException("You can't send a friend request to this user.");
        }

        var now = dateTimeProvider.UtcNow;
        var existing = await friendRequestRepository.GetBetweenAsync(currentUser.UserId, target.Id, cancellationToken);

        if (existing is null)
        {
            var created = new FriendRequest
            {
                RequesterId = currentUser.UserId,
                AddresseeId = target.Id,
                Status = FriendRequestStatus.Pending,
                CreatedAt = now
            };

            friendRequestRepository.Add(created);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new FriendRequestDto(created.Id, target.ToPublicProfileDto(), created.Status, created.CreatedAt);
        }

        if (existing.Status == FriendRequestStatus.Accepted)
        {
            throw new ConflictException("You are already friends with this user.");
        }

        if (existing.Status == FriendRequestStatus.Pending)
        {
            if (existing.RequesterId == currentUser.UserId)
            {
                throw new ConflictException("Friend request already sent.");
            }

            // The other user already asked us first — treat this as accepting their request.
            existing.Status = FriendRequestStatus.Accepted;
            existing.RespondedAt = now;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new FriendRequestDto(existing.Id, target.ToPublicProfileDto(), existing.Status, existing.CreatedAt);
        }

        // Previously declined — re-open it as a fresh request from this caller.
        existing.RequesterId = currentUser.UserId;
        existing.AddresseeId = target.Id;
        existing.Status = FriendRequestStatus.Pending;
        existing.CreatedAt = now;
        existing.RespondedAt = null;
        existing.RequesterFavorited = false;
        existing.AddresseeFavorited = false;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new FriendRequestDto(existing.Id, target.ToPublicProfileDto(), existing.Status, existing.CreatedAt);
    }
}
