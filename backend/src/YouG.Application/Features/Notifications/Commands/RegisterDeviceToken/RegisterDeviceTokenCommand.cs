using MediatR;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Notifications.Commands.RegisterDeviceToken;

public record RegisterDeviceTokenCommand(string Token, DevicePlatform Platform) : IRequest;
