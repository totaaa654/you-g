using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Profile.Dtos;

namespace YouG.Application.Features.Profile.Queries.SearchUsers;

public class SearchUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<SearchUsersQuery, SearchUsersResultDto>
{
    public async Task<SearchUsersResultDto> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await userRepository.SearchByUsernameAsync(
            request.Query, request.Page, request.PageSize, cancellationToken);

        var dtos = users
            .Select(u => new PublicProfileDto(u.Id, u.Username, u.DisplayName, u.Bio, u.ProfilePictureUrl, u.FriendCode))
            .ToList();

        return new SearchUsersResultDto(dtos, request.Page, request.PageSize, totalCount);
    }
}
