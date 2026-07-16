using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Dtos;

namespace YouG.Application.Features.Availability.Queries.GetMyAvailabilityInstances;

public class GetMyAvailabilityInstancesQueryHandler(
    IAvailabilityInstanceRepository instanceRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyAvailabilityInstancesQuery, List<AvailabilityInstanceDto>>
{
    public async Task<List<AvailabilityInstanceDto>> Handle(GetMyAvailabilityInstancesQuery request, CancellationToken cancellationToken)
    {
        var instances = await instanceRepository.GetForUserInRangeAsync(
            currentUser.UserId, request.From, request.To, cancellationToken);

        return instances
            .Select(i => new AvailabilityInstanceDto(i.Date, i.StartTime, i.Status))
            .ToList();
    }
}
