using YouG.Domain.Enums;

namespace YouG.API.Contracts.Notifications;

public record RegisterDeviceTokenRequest(string Token, DevicePlatform Platform);
