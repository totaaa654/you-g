using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Events.Commands.VoteTimeOption;

public class VoteTimeOptionCommandHandler(
    IEventRepository eventRepository,
    IEventTimeOptionRepository timeOptionRepository,
    IEventTimeVoteRepository voteRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<VoteTimeOptionCommand>
{
    public async Task Handle(VoteTimeOptionCommand request, CancellationToken cancellationToken)
    {
        await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var option = await timeOptionRepository.GetByIdAsync(request.OptionId, cancellationToken);
        if (option is null || option.EventId != request.EventId)
        {
            throw new NotFoundException("Time option not found.");
        }

        var existingVote = await voteRepository.GetAsync(request.OptionId, currentUser.UserId, cancellationToken);
        if (existingVote is not null)
        {
            return; // idempotent — voting again just confirms (docs/04-API-DESIGN.md Section 2.7)
        }

        voteRepository.Add(new EventTimeVote
        {
            EventTimeOptionId = request.OptionId,
            UserId = currentUser.UserId,
            CreatedAt = dateTimeProvider.UtcNow
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
