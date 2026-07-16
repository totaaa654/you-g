namespace YouG.Application.Features.Availability.Dtos;

public record OverlapResultDto(Guid GroupId, List<OverlapWindowDto> Windows);

public record OverlapWindowDto(
    DateOnly Date,
    TimeOnly StartTime,
    List<Guid> AvailableUserIds,
    int AvailableCount,
    int TotalMembers,
    List<Guid> MaybeUserIds);
