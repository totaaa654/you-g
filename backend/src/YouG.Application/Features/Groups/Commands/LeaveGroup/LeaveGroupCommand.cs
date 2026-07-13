using MediatR;

namespace YouG.Application.Features.Groups.Commands.LeaveGroup;

public record LeaveGroupCommand(Guid GroupId) : IRequest;
