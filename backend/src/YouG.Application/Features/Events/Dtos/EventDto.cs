using YouG.Domain.Enums;

namespace YouG.Application.Features.Events.Dtos;

public record EventDto(
    Guid Id,
    Guid GroupId,
    Guid CreatedByUserId,
    string Title,
    string? Description,
    int? MaxAttendees,
    EventStatus Status,
    Guid? ConfirmedTimeOptionId,
    Guid? ConfirmedLocationOptionId,
    DateTimeOffset CreatedAt);
