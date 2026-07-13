using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand(Guid GroupId, string Name, string? Description) : IRequest<GroupDto>;
