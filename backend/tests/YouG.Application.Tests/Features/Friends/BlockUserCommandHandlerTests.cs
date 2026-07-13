using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Friends.Commands.BlockUser;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class BlockUserCommandHandlerTests
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
    public async Task Handle_ExistingFriendship_RemovesFriendshipAndBlocks()
    {
        var caller = MakeUser();
        var target = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);
        users.Add(target);

        var friendRequests = new FakeFriendRequestRepository();
        friendRequests.Add(new FriendRequest
        {
            RequesterId = caller.Id, AddresseeId = target.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow
        });

        var blockedUsers = new FakeBlockedUserRepository();
        var handler = new BlockUserCommandHandler(
            blockedUsers, friendRequests, users, new FakeUnitOfWork(), new FakeCurrentUserService(caller.Id),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await handler.Handle(new BlockUserCommand(target.Id), CancellationToken.None);

        Assert.Empty(friendRequests.FriendRequests);
        Assert.Single(blockedUsers.BlockedUsers);
    }

    [Fact]
    public async Task Handle_SelfBlock_ThrowsConflictException()
    {
        var caller = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);

        var handler = new BlockUserCommandHandler(
            new FakeBlockedUserRepository(), new FakeFriendRequestRepository(), users, new FakeUnitOfWork(),
            new FakeCurrentUserService(caller.Id), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new BlockUserCommand(caller.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TargetNotFound_ThrowsNotFoundException()
    {
        var caller = MakeUser();
        var users = new FakeUserRepository();
        users.Add(caller);

        var handler = new BlockUserCommandHandler(
            new FakeBlockedUserRepository(), new FakeFriendRequestRepository(), users, new FakeUnitOfWork(),
            new FakeCurrentUserService(caller.Id), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new BlockUserCommand(Guid.CreateVersion7()), CancellationToken.None));
    }
}
