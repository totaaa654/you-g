using MediatR;

namespace YouG.Application.Features.Groups.Commands.RemoveMember;

public record RemoveMemberCommand(Guid GroupId, Guid TargetUserId) : IRequest;
