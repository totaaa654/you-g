using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.RespondToGroupJoinRequest;

public class RespondToGroupJoinRequestCommandHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupJoinRequestRepository joinRequestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<RespondToGroupJoinRequestCommand>
{
    public async Task Handle(RespondToGroupJoinRequestCommand request, CancellationToken cancellationToken)
    {
        var callerMembership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);
        GroupAuthorization.RequireAdmin(callerMembership);

        var joinRequest = await joinRequestRepository.GetByIdAsync(request.JoinRequestId, cancellationToken);

        if (joinRequest is null || joinRequest.GroupId != request.GroupId)
        {
            throw new NotFoundException("Join request not found.");
        }

        if (joinRequest.Status != GroupJoinRequestStatus.Pending)
        {
            throw new ConflictException("This join request has already been responded to.");
        }

        var now = dateTimeProvider.UtcNow;
        joinRequest.Status = request.Status;
        joinRequest.RespondedAt = now;

        if (request.Status == GroupJoinRequestStatus.Accepted)
        {
            var existingMembership = await groupMemberRepository.GetByGroupAndUserAsync(
                request.GroupId, joinRequest.UserId, cancellationToken);

            if (existingMembership is null)
            {
                groupMemberRepository.Add(new GroupMember
                {
                    GroupId = request.GroupId,
                    UserId = joinRequest.UserId,
                    Role = GroupRole.Member,
                    JoinedAt = now
                });
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
