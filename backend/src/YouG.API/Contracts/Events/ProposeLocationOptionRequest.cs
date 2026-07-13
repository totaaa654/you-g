namespace YouG.API.Contracts.Events;

public record ProposeLocationOptionRequest(string Name, string? Address, double Latitude, double Longitude);
