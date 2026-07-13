using YouG.Application.Features.Friends.Queries.GetMyFriends;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class GetMyFriendsQueryHandlerTests
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
    public async Task Handle_ReturnsAcceptedFriendsWithCorrectFavoriteFlag()
    {
        var caller = MakeUser();
        var friendAsAddressee = MakeUser();
        var friendAsRequester = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(friendAsAddressee);
        users.Add(friendAsRequester);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = friendAsAddressee.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow, RequesterFavorited = true
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = friendAsRequester.Id, AddresseeId = caller.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow, AddresseeFavorited = false
        });
        // A pending request should not show up as a friend.
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = MakeUser().Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new GetMyFriendsQueryHandler(friendRequests, users, new FakeCurrentUserService(caller.Id));

        var result = await handler.Handle(new GetMyFriendsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Profile.Id == friendAsAddressee.Id && f.IsFavorite);
        Assert.Contains(result, f => f.Profile.Id == friendAsRequester.Id && !f.IsFavorite);
    }
}
