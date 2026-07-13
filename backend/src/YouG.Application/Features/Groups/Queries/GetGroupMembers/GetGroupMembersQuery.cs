using MediatR;
using YouG.Application.Features.Groups.Dtos;

namespace YouG.Application.Features.Groups.Queries.GetGroupMembers;

public record GetGroupMembersQuery(Guid GroupId) : IRequest<List<GroupMemberDto>>;
