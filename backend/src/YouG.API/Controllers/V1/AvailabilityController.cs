using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Availability;
using YouG.Application.Features.Availability.Commands.CreateAvailabilityRule;
using YouG.Application.Features.Availability.Commands.DeleteAvailabilityRule;
using YouG.Application.Features.Availability.Commands.UpsertAvailabilityInstances;
using YouG.Application.Features.Availability.Dtos;
using YouG.Application.Features.Availability.Queries.GetMyAvailabilityInstances;
using YouG.Application.Features.Availability.Queries.GetMyAvailabilityRules;

namespace YouG.API.Controllers.V1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/availability")]
public class AvailabilityController(ISender sender) : ControllerBase
{
    [HttpGet("me/rules")]
    [ProducesResponseType<List<AvailabilityRuleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AvailabilityRuleDto>>> GetMyRules(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyAvailabilityRulesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("me/rules")]
    [ProducesResponseType<AvailabilityRuleDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AvailabilityRuleDto>> CreateRule(CreateAvailabilityRuleRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateAvailabilityRuleCommand(request.DayOfWeek, request.Daypart, request.Status, request.EffectiveFrom, request.EffectiveUntil),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("me/rules/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRule(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteAvailabilityRuleCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/instances")]
    [ProducesResponseType<List<AvailabilityInstanceDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AvailabilityInstanceDto>>> GetMyInstances(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyAvailabilityInstancesQuery(from, to), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("me/instances")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertInstances(List<UpsertAvailabilityInstanceRequest> request, CancellationToken cancellationToken)
    {
        var instances = request.Select(r => new AvailabilityInstanceUpsert(r.Date, r.Daypart, r.Status)).ToList();
        await sender.Send(new UpsertAvailabilityInstancesCommand(instances), cancellationToken);
        return NoContent();
    }
}
