using YouG.Application.Features.Events.Dtos;
using YouG.Application.Features.Groups.Dtos;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Search.Dtos;

/// <summary>Only the array matching the requested `SearchScope` is populated — the others are empty.</summary>
public record SearchResultDto(List<PublicProfileDto> Friends, List<GroupDto> Groups, List<EventDto> Events);
