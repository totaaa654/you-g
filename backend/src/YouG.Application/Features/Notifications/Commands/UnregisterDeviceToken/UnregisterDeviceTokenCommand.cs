using MediatR;

namespace YouG.Application.Features.Notifications.Commands.UnregisterDeviceToken;

public record UnregisterDeviceTokenCommand(string Token) : IRequest;
