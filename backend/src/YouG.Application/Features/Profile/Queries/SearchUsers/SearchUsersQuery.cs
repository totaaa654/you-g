using MediatR;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Queries.SearchUsers;

public record SearchUsersQuery(string Query, int Page, int PageSize) : IRequest<SearchUsersResultDto>;
