using MediatR;

namespace YouG.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest;
