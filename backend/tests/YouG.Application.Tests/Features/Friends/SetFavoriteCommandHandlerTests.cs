using YouG.Application.Features.Friends.Commands.SetFavorite;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class SetFavoriteCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallerIsRequester_SetsRequesterFavorited()
    {
        var callerId = Guid.CreateVersion7();
        var friendId = Guid.CreateVersion7();

        var friendRequests = new FakeFriendRequestRepository();
        var friendship = new FriendRequest
        {
            RequesterId = callerId, AddresseeId = friendId,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        };
        friendRequests.Add(friendship);

        var handler = new SetFavoriteCommandHandler(friendRequests, new FakeUnitOfWork(), new FakeCurrentUserService(callerId));

        await handler.Handle(new SetFavoriteCommand(friendId, true), CancellationToken.None);

        Assert.True(friendship.RequesterFavorited);
        Assert.False(friendship.AddresseeFavorited);
    }

    [Fact]
    public async Task Handle_CallerIsAddressee_SetsAddresseeFavorited()
    {
        var callerId = Guid.CreateVersion7();
        var friendId = Guid.CreateVersion7();

        var friendRequests = new FakeFriendRequestRepository();
        var friendship = new FriendRequest
        {
            RequesterId = friendId, AddresseeId = callerId,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        };
        friendRequests.Add(friendship);

        var handler = new SetFavoriteCommandHandler(friendRequests, new FakeUnitOfWork(), new FakeCurrentUserService(callerId));

        await handler.Handle(new SetFavoriteCommand(friendId, true), CancellationToken.None);

        Assert.True(friendship.AddresseeFavorited);
        Assert.False(friendship.RequesterFavorited);
    }
}
