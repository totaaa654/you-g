using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Events.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Events.Commands.ProposeLocationOption;

public class ProposeLocationOptionCommandHandler(
    IEventRepository eventRepository,
    IEventLocationOptionRepository locationOptionRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<ProposeLocationOptionCommand, EventLocationOptionDto>
{
    public async Task<EventLocationOptionDto> Handle(ProposeLocationOptionCommand request, CancellationToken cancellationToken)
    {
        await EventAuthorization.RequireGroupMembershipAsync(
            eventRepository, groupMemberRepository, request.EventId, currentUser.UserId, cancellationToken);

        var option = new EventLocationOption
        {
            EventId = request.EventId,
            Name = request.Name,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ProposedByUserId = currentUser.UserId,
            CreatedAt = dateTimeProvider.UtcNow
        };

        locationOptionRepository.Add(option);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EventLocationOptionDto(
            option.Id, option.Name, option.Address, option.Latitude, option.Longitude,
            option.ProposedByUserId, VoteCount: 0, HasCurrentUserVoted: false);
    }
}
