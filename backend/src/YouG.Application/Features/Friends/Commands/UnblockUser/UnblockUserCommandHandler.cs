using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Friends.Commands.UnblockUser;

public class UnblockUserCommandHandler(
    IBlockedUserRepository blockedUserRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<UnblockUserCommand>
{
    public async Task Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var block = await blockedUserRepository.GetAsync(currentUser.UserId, request.BlockedUserId, cancellationToken)
            ?? throw new NotFoundException("Block not found.");

        blockedUserRepository.Remove(block);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
