using MediatR;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.ChangeMemberRole;

public record ChangeMemberRoleCommand(Guid GroupId, Guid TargetUserId, GroupRole NewRole) : IRequest;
