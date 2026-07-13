using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Friends.Commands.SendFriendRequest;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class SendFriendRequestCommandHandlerTests
{
    private static User MakeUser(Guid? id = null) => new()
    {
        Id = id ?? Guid.CreateVersion7(),
        Email = $"{Guid.NewGuid()}@example.com",
        Username = $"user{Guid.NewGuid():N}"[..12],
        FriendCode = $"YG-{Guid.NewGuid():N}"[..9].ToUpperInvariant(),
        DisplayName = "Test User",
        TimeZoneId = "UTC"
    };

    private static SendFriendRequestCommandHandler MakeHandler(
        Guid callerId,
        FakeFriendRequestRepository friendRequests,
        FakeBlockedUserRepository blockedUsers,
        FakeUserRepository users) =>
        new(friendRequests, blockedUsers, users, new FakeUnitOfWork(), new FakeCurrentUserService(callerId),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow));

    [Fact]
    public async Task Handle_NoExistingRelationship_CreatesPendingRequest()
    {
        var caller = MakeUser();
        var target = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(target);

        var friendRequests = new FakeFriendRequestRepository();
        var handler = MakeHandler(caller.Id, friendRequests, new FakeBlockedUserRepository(), users);

        var result = await handler.Handle(new SendFriendRequestCommand(target.Id, null), CancellationToken.None);

        Assert.Equal(FriendRequestStatus.Pending, result.Status);
        Assert.Single(friendRequests.FriendRequests);
    }

    [Fact]
    public async Task Handle_ReverseRequestAlreadyPending_AutoAccepts()
    {
        var caller = MakeUser();
        var target = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(target);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = target.Id,
            AddresseeId = caller.Id,
            Status = FriendRequestStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = MakeHandler(caller.Id, friendRequests, new FakeBlockedUserRepository(), users);

        var result = await handler.Handle(new SendFriendRequestCommand(target.Id, null), CancellationToken.None);

        Assert.Equal(FriendRequestStatus.Accepted, result.Status);
    }

    [Fact]
    public async Task Handle_AlreadyFriends_ThrowsConflictException()
    {
        var caller = MakeUser();
        var target = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(target);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id,
            AddresseeId = target.Id,
            Status = FriendRequestStatus.Accepted,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = MakeHandler(caller.Id, friendRequests, new FakeBlockedUserRepository(), users);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new SendFriendRequestCommand(target.Id, null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SelfFriendRequest_ThrowsConflictException()
    {
        var caller = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);

        var handler = MakeHandler(caller.Id, new FakeFriendRequestRepository(), new FakeBlockedUserRepository(), users);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new SendFriendRequestCommand(caller.Id, null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_BlockedRelationshipExists_ThrowsForbiddenException()
    {
        var caller = MakeUser();
        var target = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(target);

        var blockedUsers = new FakeBlockedUserRepository();
        blockedUsers.Add(new BlockedUser { BlockerId = target.Id, BlockedId = caller.Id, CreatedAt = DateTimeOffset.UtcNow });

        var handler = MakeHandler(caller.Id, new FakeFriendRequestRepository(), blockedUsers, users);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(new SendFriendRequestCommand(target.Id, null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TargetNotFound_ThrowsNotFoundException()
    {
        var caller = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);

        var handler = MakeHandler(caller.Id, new FakeFriendRequestRepository(), new FakeBlockedUserRepository(), users);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new SendFriendRequestCommand(Guid.CreateVersion7(), null), CancellationToken.None));
    }
}
