using MediatR;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;
