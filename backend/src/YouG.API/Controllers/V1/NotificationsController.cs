using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Notifications;
using YouG.Application.Features.Notifications.Commands.MarkAllNotificationsRead;
using YouG.Application.Features.Notifications.Commands.MarkNotificationRead;
using YouG.Application.Features.Notifications.Commands.RegisterDeviceToken;
using YouG.Application.Features.Notifications.Commands.UnregisterDeviceToken;
using YouG.Application.Features.Notifications.Dtos;
using YouG.Application.Features.Notifications.Queries.GetMyNotifications;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<NotificationDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NotificationDto>>> GetMyNotifications(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyNotificationsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new MarkNotificationReadCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        await sender.Send(new MarkAllNotificationsReadCommand(), cancellationToken);
        return NoContent();
    }

    [HttpPost("device-tokens")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterDeviceToken(RegisterDeviceTokenRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new RegisterDeviceTokenCommand(request.Token, request.Platform), cancellationToken);
        return NoContent();
    }

    [HttpDelete("device-tokens")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnregisterDeviceToken(
        [FromQuery] string token, CancellationToken cancellationToken)
    {
        await sender.Send(new UnregisterDeviceTokenCommand(token), cancellationToken);
        return NoContent();
    }
}
