using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouG.API.Contracts.Auth;
using YouG.Application.Features.Auth.Commands.ForgotPassword;
using YouG.Application.Features.Auth.Commands.Login;
using YouG.Application.Features.Auth.Commands.Logout;
using YouG.Application.Features.Auth.Commands.Refresh;
using YouG.Application.Features.Auth.Commands.Register;
using YouG.Application.Features.Auth.Commands.ResetPassword;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResultDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(request.Email, request.Password, request.Username, request.DisplayName, request.TimeZoneId),
            cancellationToken);

        // No GET-by-id user endpoint exists yet (Profile feature) to point Location at, so this is
        // a plain 201 rather than CreatedAtAction/CreatedAtRoute.
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResultDto>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AuthResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResultDto>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        return NoContent();
    }

    // Always 204 whether or not the email is registered — distinguishing the two would let a
    // caller enumerate accounts (see ForgotPasswordCommandHandler for the same rationale).
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return NoContent();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ResetPasswordCommand(request.Email, request.Code, request.NewPassword), cancellationToken);
        return NoContent();
    }
}
