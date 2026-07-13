using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Groups.Commands.RemoveMember;

public class RemoveMemberCommandHandler(
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<RemoveMemberCommand>
{
    public async Task Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var callerMembership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);
        GroupAuthorization.RequireAdmin(callerMembership);

        if (request.TargetUserId == currentUser.UserId)
        {
            throw new ConflictException("Use the leave-group endpoint to remove yourself.");
        }

        var targetMembership = await groupMemberRepository.GetByGroupAndUserAsync(
            request.GroupId, request.TargetUserId, cancellationToken)
            ?? throw new NotFoundException("This user is not a member of the group.");

        groupMemberRepository.Remove(targetMembership);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
