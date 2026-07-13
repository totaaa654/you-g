using MediatR;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResultDto>;
