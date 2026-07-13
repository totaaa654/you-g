using YouG.Application.Features.Search;
using YouG.Application.Features.Search.Queries.Search;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Search;

public class SearchQueryHandlerTests
{
    private static User MakeUser(string username, string displayName) => new()
    {
        Email = $"{username}@example.com",
        Username = username,
        FriendCode = $"YG-{username}",
        DisplayName = displayName,
        TimeZoneId = "UTC"
    };

    private static SearchQueryHandler MakeHandler(
        Guid callerId,
        FakeFriendRequestRepository friendRequests,
        FakeGroupMemberRepository groupMembers,
        FakeEventRepository events,
        FakeUserRepository users) =>
        new(friendRequests, groupMembers, events, users, new FakeCurrentUserService(callerId));

    [Fact]
    public async Task Handle_FriendsScope_ReturnsOnlyMatchingAcceptedFriends()
    {
        var caller = MakeUser("caller", "Caller");
        var matchingFriend = MakeUser("mayag", "Maya");
        var nonMatchingFriend = MakeUser("bobb", "Bob");
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(matchingFriend);
        users.Add(nonMatchingFriend);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = matchingFriend.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = nonMatchingFriend.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = MakeHandler(caller.Id, friendRequests, new FakeGroupMemberRepository(new FakeGroupRepository()), new FakeEventRepository(), users);

        var result = await handler.Handle(new SearchQuery("maya", SearchScope.Friends), CancellationToken.None);

        Assert.Single(result.Friends);
        Assert.Equal(matchingFriend.Id, result.Friends[0].Id);
        Assert.Empty(result.Groups);
        Assert.Empty(result.Events);
    }

    [Fact]
    public async Task Handle_GroupsScope_ReturnsOnlyMatchingGroupsUserBelongsTo()
    {
        var caller = MakeUser("caller", "Caller");
        var users = new FakeUserRepository();
        users.Add(caller);

        var groupRepo = new FakeGroupRepository();
        var matchingGroup = new Group { Name = "Hiking Crew", CreatedByUserId = caller.Id, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        var nonMatchingGroup = new Group { Name = "Book Club", CreatedByUserId = caller.Id, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        groupRepo.Groups.Add(matchingGroup);
        groupRepo.Groups.Add(nonMatchingGroup);

        var groupMembers = new FakeGroupMemberRepository(groupRepo);
        groupMembers.Members.Add(new GroupMember { GroupId = matchingGroup.Id, UserId = caller.Id, Role = GroupRole.Admin, JoinedAt = DateTimeOffset.UtcNow });
        groupMembers.Members.Add(new GroupMember { GroupId = nonMatchingGroup.Id, UserId = caller.Id, Role = GroupRole.Admin, JoinedAt = DateTimeOffset.UtcNow });

        var handler = MakeHandler(caller.Id, new FakeFriendRequestRepository(), groupMembers, new FakeEventRepository(), users);

        var result = await handler.Handle(new SearchQuery("hiking", SearchScope.Groups), CancellationToken.None);

        Assert.Single(result.Groups);
        Assert.Equal(matchingGroup.Id, result.Groups[0].Id);
        Assert.Empty(result.Friends);
        Assert.Empty(result.Events);
    }

    [Fact]
    public async Task Handle_EventsScope_ReturnsOnlyMatchingEventsInUsersGroups()
    {
        var caller = MakeUser("caller", "Caller");
        var users = new FakeUserRepository();
        users.Add(caller);

        var groupRepo = new FakeGroupRepository();
        var group = new Group { Name = "Group", CreatedByUserId = caller.Id, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        groupRepo.Groups.Add(group);

        var groupMembers = new FakeGroupMemberRepository(groupRepo);
        groupMembers.Members.Add(new GroupMember { GroupId = group.Id, UserId = caller.Id, Role = GroupRole.Admin, JoinedAt = DateTimeOffset.UtcNow });

        var events = new FakeEventRepository();
        var matchingEvent = new Event { GroupId = group.Id, CreatedByUserId = caller.Id, Title = "Beach Day", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        var nonMatchingEvent = new Event { GroupId = group.Id, CreatedByUserId = caller.Id, Title = "Movie Night", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        events.Events.Add(matchingEvent);
        events.Events.Add(nonMatchingEvent);

        var handler = MakeHandler(caller.Id, new FakeFriendRequestRepository(), groupMembers, events, users);

        var result = await handler.Handle(new SearchQuery("beach", SearchScope.Events), CancellationToken.None);

        Assert.Single(result.Events);
        Assert.Equal(matchingEvent.Id, result.Events[0].Id);
        Assert.Empty(result.Friends);
        Assert.Empty(result.Groups);
    }
}
