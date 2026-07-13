using YouG.Domain.Enums;

namespace YouG.API.Contracts.Availability;

public record UpsertAvailabilityInstanceRequest(DateOnly Date, Daypart Daypart, AvailabilityStatus Status);
