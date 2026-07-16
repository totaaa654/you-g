using MediatR;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Groups.Commands.RespondToGroupJoinRequest;

public record RespondToGroupJoinRequestCommand(Guid GroupId, Guid JoinRequestId, GroupJoinRequestStatus Status) : IRequest;
