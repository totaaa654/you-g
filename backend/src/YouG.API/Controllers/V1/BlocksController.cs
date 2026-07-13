using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Blocks;
using YouG.Application.Features.Friends.Commands.BlockUser;
using YouG.Application.Features.Friends.Commands.UnblockUser;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/blocks")]
public class BlocksController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Block(BlockUserRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new BlockUserCommand(request.BlockedUserId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unblock(Guid userId, CancellationToken cancellationToken)
    {
        await sender.Send(new UnblockUserCommand(userId), cancellationToken);
        return NoContent();
    }
}
