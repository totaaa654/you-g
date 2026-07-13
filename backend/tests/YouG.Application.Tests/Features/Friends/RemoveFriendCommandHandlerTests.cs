using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Friends.Commands.RemoveFriend;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class RemoveFriendCommandHandlerTests
{
    [Fact]
    public async Task Handle_AcceptedFriendship_RemovesIt()
    {
        var callerId = Guid.CreateVersion7();
        var friendId = Guid.CreateVersion7();

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = callerId, AddresseeId = friendId,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new RemoveFriendCommandHandler(friendRequests, new FakeUnitOfWork(), new FakeCurrentUserService(callerId));

        await handler.Handle(new RemoveFriendCommand(friendId), CancellationToken.None);

        Assert.Empty(friendRequests.FriendRequests);
    }

    [Fact]
    public async Task Handle_NoFriendship_ThrowsNotFoundException()
    {
        var callerId = Guid.CreateVersion7();
        var friendId = Guid.CreateVersion7();

        var handler = new RemoveFriendCommandHandler(
            new FakeFriendRequestRepository(), new FakeUnitOfWork(), new FakeCurrentUserService(callerId));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new RemoveFriendCommand(friendId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PendingRequestNotAccepted_ThrowsNotFoundException()
    {
        var callerId = Guid.CreateVersion7();
        var friendId = Guid.CreateVersion7();

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = callerId, AddresseeId = friendId,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new RemoveFriendCommandHandler(friendRequests, new FakeUnitOfWork(), new FakeCurrentUserService(callerId));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new RemoveFriendCommand(friendId), CancellationToken.None));
    }
}
