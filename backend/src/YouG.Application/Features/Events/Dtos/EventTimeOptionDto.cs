namespace YouG.Application.Features.Events.Dtos;

public record EventTimeOptionDto(
    Guid Id,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    Guid ProposedByUserId,
    int VoteCount,
    bool HasCurrentUserVoted);
