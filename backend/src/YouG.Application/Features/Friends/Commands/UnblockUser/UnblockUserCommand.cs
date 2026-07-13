using MediatR;

namespace YouG.Application.Features.Friends.Commands.UnblockUser;

public record UnblockUserCommand(Guid BlockedUserId) : IRequest;
