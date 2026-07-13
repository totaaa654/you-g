using MediatR;

namespace YouG.Application.Features.Friends.Commands.BlockUser;

public record BlockUserCommand(Guid BlockedUserId) : IRequest;
