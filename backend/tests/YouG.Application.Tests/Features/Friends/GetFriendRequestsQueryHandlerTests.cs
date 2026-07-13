using YouG.Application.Features.Friends;
using YouG.Application.Features.Friends.Queries.GetFriendRequests;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class GetFriendRequestsQueryHandlerTests
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
    public async Task Handle_Incoming_ReturnsOnlyRequestsAddressedToCaller()
    {
        var caller = MakeUser();
        var incomingRequester = MakeUser();
        var outgoingAddressee = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(incomingRequester);
        users.Add(outgoingAddressee);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = incomingRequester.Id, AddresseeId = caller.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = outgoingAddressee.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new GetFriendRequestsQueryHandler(friendRequests, users, new FakeCurrentUserService(caller.Id));

        var result = await handler.Handle(new GetFriendRequestsQuery(FriendRequestDirection.Incoming), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(incomingRequester.Id, result[0].Profile.Id);
    }

    [Fact]
    public async Task Handle_Outgoing_ReturnsOnlyRequestsSentByCaller()
    {
        var caller = MakeUser();
        var incomingRequester = MakeUser();
        var outgoingAddressee = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(incomingRequester);
        users.Add(outgoingAddressee);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = incomingRequester.Id, AddresseeId = caller.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = outgoingAddressee.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new GetFriendRequestsQueryHandler(friendRequests, users, new FakeCurrentUserService(caller.Id));

        var result = await handler.Handle(new GetFriendRequestsQuery(FriendRequestDirection.Outgoing), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(outgoingAddressee.Id, result[0].Profile.Id);
    }
}
