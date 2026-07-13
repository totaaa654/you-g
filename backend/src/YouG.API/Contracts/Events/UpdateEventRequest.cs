namespace YouG.API.Contracts.Events;

public record UpdateEventRequest(string Title, string? Description, int? MaxAttendees);
