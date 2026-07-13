namespace YouG.Application.Features.Profile.Dtos;

public record SearchUsersResultDto(List<PublicProfileDto> Users, int Page, int PageSize, int TotalCount);
