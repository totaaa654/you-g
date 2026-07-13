using MediatR;
using YouG.Application.Features.Search.Dtos;

namespace YouG.Application.Features.Search.Queries.Search;

public record SearchQuery(string Q, SearchScope Scope) : IRequest<SearchResultDto>;
