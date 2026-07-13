using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.Application.Features.Search;
using YouG.Application.Features.Search.Dtos;
using YouG.Application.Features.Search.Queries.Search;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/search")]
public class SearchController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<SearchResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResultDto>> Search(
        [FromQuery] string q, [FromQuery] SearchScope type, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SearchQuery(q, type), cancellationToken);
        return Ok(result);
    }
}
