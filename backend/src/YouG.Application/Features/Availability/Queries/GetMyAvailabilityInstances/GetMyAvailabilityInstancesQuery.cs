using MediatR;
using YouG.Application.Features.Availability.Dtos;

namespace YouG.Application.Features.Availability.Queries.GetMyAvailabilityInstances;

public record GetMyAvailabilityInstancesQuery(DateOnly From, DateOnly To) : IRequest<List<AvailabilityInstanceDto>>;
