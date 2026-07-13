using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.LeaveGroup;

public class LeaveGroupCommandHandler(
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<LeaveGroupCommand>
{
    public async Task Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var membership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);

        if (membership.Role == GroupRole.Admin)
        {
            var members = await groupMemberRepository.GetMembersByGroupIdAsync(request.GroupId, cancellationToken);
            var otherAdminExists = members.Any(m => m.UserId != currentUser.UserId && m.Role == GroupRole.Admin);

            if (!otherAdminExists && members.Count > 1)
            {
                throw new ConflictException(
                    "You're the only admin. Promote another member to admin before leaving.");
            }
        }

        groupMemberRepository.Remove(membership);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
