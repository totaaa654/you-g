using MediatR;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string Username,
    string DisplayName,
    string TimeZoneId) : IRequest<AuthResultDto>;
