using MediatR;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Events.Commands.RetractTimeVote;

public class RetractTimeVoteCommandHandler(
    IEventRepository eventRepository,
    IEventTimeVoteRepository voteRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<RetractTimeVoteCommand>
{
    public async Task Handle(RetractTimeVoteCommand request, CancellationToken cancellationToken)
    {
        await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var existingVote = await voteRepository.GetAsync(request.OptionId, currentUser.UserId, cancellationToken);
        if (existingVote is null)
        {
            return; // already retracted (or never voted) — no-op, not an error
        }

        voteRepository.Remove(existingVote);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
