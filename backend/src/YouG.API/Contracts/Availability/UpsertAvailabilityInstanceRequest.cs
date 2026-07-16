using YouG.Domain.Enums;

namespace YouG.API.Contracts.Availability;

public record UpsertAvailabilityInstanceRequest(DateOnly Date, TimeOnly StartTime, AvailabilityStatus Status);
