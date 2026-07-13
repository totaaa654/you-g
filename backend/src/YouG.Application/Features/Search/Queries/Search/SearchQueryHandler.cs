using MediatR;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Friends;
using YouG.Application.Features.Groups.Dtos;
using YouG.Application.Features.Search.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Search.Queries.Search;

public class SearchQueryHandler(
    IFriendRequestRepository friendRequestRepository,
    IGroupMemberRepository groupMemberRepository,
    IEventRepository eventRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<SearchQuery, SearchResultDto>
{
    private static readonly List<Profile.Dtos.PublicProfileDto> EmptyFriends = [];
    private static readonly List<GroupDto> EmptyGroups = [];
    private static readonly List<Events.Dtos.EventDto> EmptyEvents = [];

    public async Task<SearchResultDto> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        return request.Scope switch
        {
            SearchScope.Friends => new SearchResultDto(await SearchFriends(request.Q, cancellationToken), EmptyGroups, EmptyEvents),
            SearchScope.Groups => new SearchResultDto(EmptyFriends, await SearchGroups(request.Q, cancellationToken), EmptyEvents),
            SearchScope.Events => new SearchResultDto(EmptyFriends, EmptyGroups, await SearchEvents(request.Q, cancellationToken)),
            _ => new SearchResultDto(EmptyFriends, EmptyGroups, EmptyEvents)
        };
    }

    private async Task<List<Profile.Dtos.PublicProfileDto>> SearchFriends(string query, CancellationToken cancellationToken)
    {
        var friendships = await friendRequestRepository.GetAcceptedForUserAsync(currentUser.UserId, cancellationToken);
        var friendIds = friendships
            .Select(f => f.RequesterId == currentUser.UserId ? f.AddresseeId : f.RequesterId)
            .ToList();
        var users = await userRepository.GetByIdsAsync(friendIds, cancellationToken);

        return users
            .Where(u => Matches(u.Username, query) || Matches(u.DisplayName, query))
            .Select(u => u.ToPublicProfileDto())
            .ToList();
    }

    private async Task<List<GroupDto>> SearchGroups(string query, CancellationToken cancellationToken)
    {
        var groups = await groupMemberRepository.GetGroupsForUserAsync(currentUser.UserId, cancellationToken);
        var matches = groups.Where(g => Matches(g.Name, query)).ToList();

        var result = new List<GroupDto>(matches.Count);
        foreach (var group in matches)
        {
            var memberCount = await groupMemberRepository.CountMembersAsync(group.Id, cancellationToken);
            result.Add(new GroupDto(group.Id, group.Name, group.Description, group.PictureUrl, group.CreatedByUserId, memberCount, group.CreatedAt));
        }

        return result;
    }

    private async Task<List<Events.Dtos.EventDto>> SearchEvents(string query, CancellationToken cancellationToken)
    {
        var groups = await groupMemberRepository.GetGroupsForUserAsync(currentUser.UserId, cancellationToken);
        var events = await eventRepository.GetByGroupIdsAsync(groups.Select(g => g.Id).ToList(), cancellationToken);

        return events
            .Where(e => Matches(e.Title, query))
            .Select(e => new Events.Dtos.EventDto(
                e.Id, e.GroupId, e.CreatedByUserId, e.Title, e.Description, e.MaxAttendees,
                e.Status, e.ConfirmedTimeOptionId, e.ConfirmedLocationOptionId, e.CreatedAt))
            .ToList();
    }

    private static bool Matches(string value, string query) =>
        value.Contains(query, StringComparison.OrdinalIgnoreCase);
}
