using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Commands.CreateInviteLink;

public record CreateInviteLinkCommand(Guid GroupId) : IRequest<InviteLinkDto>;
