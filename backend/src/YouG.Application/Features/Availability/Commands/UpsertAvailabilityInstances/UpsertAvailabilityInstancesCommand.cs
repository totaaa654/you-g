using MediatR;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Commands.UpsertAvailabilityInstances;

public record AvailabilityInstanceUpsert(DateOnly Date, TimeOnly StartTime, AvailabilityStatus Status);

public record UpsertAvailabilityInstancesCommand(List<AvailabilityInstanceUpsert> Instances) : IRequest;
