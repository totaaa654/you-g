using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.ChangeMemberRole;

public class ChangeMemberRoleCommandHandler(
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<ChangeMemberRoleCommand>
{
    public async Task Handle(ChangeMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var callerMembership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);
        GroupAuthorization.RequireAdmin(callerMembership);

        var targetMembership = await groupMemberRepository.GetByGroupAndUserAsync(
            request.GroupId, request.TargetUserId, cancellationToken)
            ?? throw new NotFoundException("This user is not a member of the group.");

        if (targetMembership.Role == GroupRole.Admin && request.NewRole == GroupRole.Member)
        {
            var members = await groupMemberRepository.GetMembersByGroupIdAsync(request.GroupId, cancellationToken);
            var otherAdminExists = members.Any(m => m.UserId != request.TargetUserId && m.Role == GroupRole.Admin);

            if (!otherAdminExists)
            {
                throw new ConflictException("Promote another member to admin before demoting the only admin.");
            }
        }

        targetMembership.Role = request.NewRole;
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
