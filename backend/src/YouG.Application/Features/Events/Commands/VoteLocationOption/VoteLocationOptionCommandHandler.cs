using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Events.Commands.VoteLocationOption;

public class VoteLocationOptionCommandHandler(
    IEventRepository eventRepository,
    IEventLocationOptionRepository locationOptionRepository,
    IEventLocationVoteRepository voteRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<VoteLocationOptionCommand>
{
    public async Task Handle(VoteLocationOptionCommand request, CancellationToken cancellationToken)
    {
        await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var option = await locationOptionRepository.GetByIdAsync(request.OptionId, cancellationToken);
        if (option is null || option.EventId != request.EventId)
        {
            throw new NotFoundException("Location option not found.");
        }

        var existingVote = await voteRepository.GetAsync(request.OptionId, currentUser.UserId, cancellationToken);
        if (existingVote is not null)
        {
            return;
        }

        voteRepository.Add(new EventLocationVote
        {
            EventLocationOptionId = request.OptionId,
            UserId = currentUser.UserId,
            CreatedAt = dateTimeProvider.UtcNow
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
