using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetGroupById;

public record GetGroupByIdQuery(Guid GroupId) : IRequest<GroupDto>;
