using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Dtos;

public record AvailabilityInstanceDto(DateOnly Date, TimeOnly StartTime, AvailabilityStatus Status);
