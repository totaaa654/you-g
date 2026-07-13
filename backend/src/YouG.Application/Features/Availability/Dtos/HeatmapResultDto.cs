using YouG.Domain.Enums;

namespace YouG.Application.Features.Availability.Dtos;

public record HeatmapResultDto(Guid GroupId, DateOnly From, DateOnly To, List<HeatmapCellDto> Cells, int TotalMembers);

public record HeatmapCellDto(DateOnly Date, Daypart Daypart, int AvailableCount);
