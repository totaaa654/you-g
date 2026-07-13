using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Events.Commands.ProposeTimeOption;

public class ProposeTimeOptionCommandHandler(
    IEventRepository eventRepository,
    IEventTimeOptionRepository timeOptionRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<ProposeTimeOptionCommand, EventTimeOptionDto>
{
    public async Task<EventTimeOptionDto> Handle(ProposeTimeOptionCommand request, CancellationToken cancellationToken)
    {
        await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var option = new EventTimeOption
        {
            EventId = request.EventId,
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            ProposedByUserId = currentUser.UserId,
            CreatedAt = dateTimeProvider.UtcNow
        };

        timeOptionRepository.Add(option);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EventTimeOptionDto(option.Id, option.StartUtc, option.EndUtc, option.ProposedByUserId, VoteCount: 0, HasCurrentUserVoted: false);
    }
}
