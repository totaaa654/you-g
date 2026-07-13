using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetMyGroups;

public record GetMyGroupsQuery : IRequest<List<GroupDto>>;
