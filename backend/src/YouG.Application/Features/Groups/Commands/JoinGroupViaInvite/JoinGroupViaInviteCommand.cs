using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Commands.JoinGroupViaInvite;

public record JoinGroupViaInviteCommand(string Code) : IRequest<GroupDto>;
