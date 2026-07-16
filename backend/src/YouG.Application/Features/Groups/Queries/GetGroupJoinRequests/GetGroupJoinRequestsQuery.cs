using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetGroupJoinRequests;

public record GetGroupJoinRequestsQuery(Guid GroupId) : IRequest<List<GroupJoinRequestDto>>;
