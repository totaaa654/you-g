using MediatR;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Events.Commands.RetractLocationVote;

public class RetractLocationVoteCommandHandler(
    IEventRepository eventRepository,
    IEventLocationVoteRepository voteRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<RetractLocationVoteCommand>
{
    public async Task Handle(RetractLocationVoteCommand request, CancellationToken cancellationToken)
    {
        await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var existingVote = await voteRepository.GetAsync(request.OptionId, currentUser.UserId, cancellationToken);
        if (existingVote is null)
        {
            return;
        }

        voteRepository.Remove(existingVote);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
