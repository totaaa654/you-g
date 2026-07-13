using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Friends;
using YouG.Application.Features.Friends;
using YouG.Application.Features.Friends.Commands.RemoveFriend;
using YouG.Application.Features.Friends.Commands.RespondToFriendRequest;
using YouG.Application.Features.Friends.Commands.SendFriendRequest;
using YouG.Application.Features.Friends.Commands.SetFavorite;
using YouG.Application.Features.Friends.Dtos;
using YouG.Application.Features.Friends.Queries.GetFriendRequests;
using YouG.Application.Features.Friends.Queries.GetMutualFriends;
using YouG.Application.Features.Friends.Queries.GetMyFriends;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/friends")]
public class FriendsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<FriendDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FriendDto>>> GetMyFriends(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyFriendsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("requests")]
    [ProducesResponseType<FriendRequestDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<FriendRequestDto>> SendRequest(SendFriendRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SendFriendRequestCommand(request.AddresseeId, request.FriendCode), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("requests")]
    [ProducesResponseType<List<FriendRequestDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FriendRequestDto>>> GetRequests(
        [FromQuery] FriendRequestDirection direction, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFriendRequestsQuery(direction), cancellationToken);
        return Ok(result);
    }

    [HttpPut("requests/{id:guid}")]
    [ProducesResponseType<FriendRequestDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<FriendRequestDto>> RespondToRequest(
        Guid id, RespondToFriendRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RespondToFriendRequestCommand(id, request.Status), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveFriend(Guid userId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveFriendCommand(userId), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetFavorite(Guid userId, SetFavoriteRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new SetFavoriteCommand(userId, request.IsFavorite), cancellationToken);
        return NoContent();
    }

    [HttpGet("{userId:guid}/mutual")]
    [ProducesResponseType<List<PublicProfileDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PublicProfileDto>>> GetMutualFriends(Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMutualFriendsQuery(userId), cancellationToken);
        return Ok(result);
    }
}
