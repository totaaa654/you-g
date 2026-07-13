using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Commands.CreateGroup;

public record CreateGroupCommand(string Name, string? Description) : IRequest<GroupDto>;
