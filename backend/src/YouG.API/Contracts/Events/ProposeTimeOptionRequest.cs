namespace YouG.API.Contracts.Events;

public record ProposeTimeOptionRequest(DateTimeOffset StartUtc, DateTimeOffset EndUtc);
