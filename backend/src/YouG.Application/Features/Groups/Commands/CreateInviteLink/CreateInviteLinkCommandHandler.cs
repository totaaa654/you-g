using MediatR;
using YouG.Application.Common;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Groups.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Groups.Commands.CreateInviteLink;

public class CreateInviteLinkCommandHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupInviteLinkRepository inviteLinkRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CreateInviteLinkCommand, InviteLinkDto>
{
    // Per docs/01-PRD.md Section 8: invite links expire after 7 days by default.
    private static readonly TimeSpan LinkLifetime = TimeSpan.FromDays(7);

    public async Task<InviteLinkDto> Handle(CreateInviteLinkCommand request, CancellationToken cancellationToken)
    {
        var membership = await GroupAuthorization.RequireMembershipAsync(
            groupMemberRepository, request.GroupId, currentUser.UserId, cancellationToken);
        GroupAuthorization.RequireAdmin(membership);

        var code = await GenerateUniqueCodeAsync(cancellationToken);
        var now = dateTimeProvider.UtcNow;
        var expiresAt = now.Add(LinkLifetime);

        inviteLinkRepository.Add(new GroupInviteLink
        {
            GroupId = request.GroupId,
            Code = code,
            CreatedByUserId = currentUser.UserId,
            ExpiresAt = expiresAt,
            CreatedAt = now
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InviteLinkDto(code, expiresAt);
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = InviteCodeGenerator.Generate();
            if (!await inviteLinkRepository.ExistsByCodeAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Failed to generate a unique invite code after 5 attempts.");
    }
}
