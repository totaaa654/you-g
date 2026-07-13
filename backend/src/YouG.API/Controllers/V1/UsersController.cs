using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Users;
using YouG.Application.Features.Profile.Commands.DeleteMyAccount;
using YouG.Application.Features.Profile.Commands.UpdateMyProfile;
using YouG.Application.Features.Profile.Dtos;
using YouG.Application.Features.Profile.Queries.GetMyProfile;
using YouG.Application.Features.Profile.Queries.GetPublicProfile;
using YouG.Application.Features.Profile.Queries.SearchUsers;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfileDto>> GetMe(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyProfileQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("me")]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfileDto>> UpdateMe(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateMyProfileCommand(request.DisplayName, request.Bio, request.TimeZoneId, request.ProfilePictureUrl),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMe(CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteMyAccountCommand(), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PublicProfileDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicProfileDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPublicProfileQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType<SearchUsersResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchUsersResultDto>> Search(
        [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new SearchUsersQuery(q, page, pageSize), cancellationToken);
        return Ok(result);
    }
}
