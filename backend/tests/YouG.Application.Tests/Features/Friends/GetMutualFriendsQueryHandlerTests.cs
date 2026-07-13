using YouG.Application.Features.Friends.Queries.GetMutualFriends;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class GetMutualFriendsQueryHandlerTests
{
    private static User MakeUser() => new()
    {
        Id = Guid.CreateVersion7(),
        Email = $"{Guid.NewGuid()}@example.com",
        Username = $"user{Guid.NewGuid():N}"[..12],
        FriendCode = $"YG-{Guid.NewGuid():N}"[..9].ToUpperInvariant(),
        DisplayName = "Test User",
        TimeZoneId = "UTC"
    };

    [Fact]
    public async Task Handle_ReturnsOnlyFriendsSharedByBothUsers()
    {
        var caller = MakeUser();
        var other = MakeUser();
        var mutualFriend = MakeUser();
        var onlyCallersFriend = MakeUser();
        var onlyOthersFriend = MakeUser();

        var users = new FakeUserRepository();
        foreach (var u in new[] { caller, other, mutualFriend, onlyCallersFriend, onlyOthersFriend })
        {
            users.Add(u);
        }

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = mutualFriend.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = other.Id, AddresseeId = mutualFriend.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = onlyCallersFriend.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = other.Id, AddresseeId = onlyOthersFriend.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new GetMutualFriendsQueryHandler(friendRequests, users, new FakeCurrentUserService(caller.Id));

        var result = await handler.Handle(new GetMutualFriendsQuery(other.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(mutualFriend.Id, result[0].Id);
    }
}
