namespace YouG.Application.Features.Events.Dtos;

public record EventLocationOptionDto(
    Guid Id,
    string Name,
    string? Address,
    double Latitude,
    double Longitude,
    Guid ProposedByUserId,
    int VoteCount,
    bool HasCurrentUserVoted);
