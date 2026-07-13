using MediatR;
using YouG.Application.Features.Events.Dtos;

namespace YouG.Application.Features.Events.Commands.CreateEvent;

/// <summary>
/// If a fixed time AND location are both supplied, the event is created already Confirmed
/// (the organizer's choice becomes a single pre-confirmed option — no voting needed). If either
/// is omitted, the event starts Proposed so members can propose/vote on options before someone
/// calls Confirm (docs/03-DATABASE.md Section 3.10 design note).
/// </summary>
public record CreateEventCommand(
    Guid GroupId,
    string Title,
    string? Description,
    int? MaxAttendees,
    DateTimeOffset? InitialStartUtc,
    DateTimeOffset? InitialEndUtc,
    string? InitialLocationName,
    string? InitialLocationAddress,
    double? InitialLocationLatitude,
    double? InitialLocationLongitude) : IRequest<EventDto>;
