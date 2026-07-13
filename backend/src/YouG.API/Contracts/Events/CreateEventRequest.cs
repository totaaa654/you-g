namespace YouG.API.Contracts.Events;

public record CreateEventRequest(
    string Title,
    string? Description,
    int? MaxAttendees,
    DateTimeOffset? InitialStartUtc,
    DateTimeOffset? InitialEndUtc,
    string? InitialLocationName,
    string? InitialLocationAddress,
    double? InitialLocationLatitude,
    double? InitialLocationLongitude);
