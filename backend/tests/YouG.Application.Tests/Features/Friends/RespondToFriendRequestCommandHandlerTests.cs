using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Friends.Commands.RespondToFriendRequest;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Friends;

public class RespondToFriendRequestCommandHandlerTests
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
    public async Task Handle_CallerIsRequester_ThrowsForbiddenException()
    {
        var requester = MakeUser();
        var addressee = MakeUser();
        var users = new FakeUserRepository();
        users.Add(requester);
        users.Add(addressee);

        var friendRequests = new FakeFriendRequestRepository();
        var request = new FriendRequest
        {
            RequesterId = requester.Id, AddresseeId = addressee.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        };
        friendRequests.Add(request);

        var handler = new RespondToFriendRequestCommandHandler(
            friendRequests, users, new FakeUnitOfWork(), new FakeCurrentUserService(requester.Id),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow), new FakeNotificationDispatcher());

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(
            new RespondToFriendRequestCommand(request.Id, FriendRequestStatus.Accepted), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NotParticipant_ThrowsNotFoundException()
    {
        var requester = MakeUser();
        var addressee = MakeUser();
        var stranger = MakeUser();
        var users = new FakeUserRepository();
        users.Add(requester);
        users.Add(addressee);
        users.Add(stranger);

        var friendRequests = new FakeFriendRequestRepository();
        var request = new FriendRequest
        {
            RequesterId = requester.Id, AddresseeId = addressee.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        };
        friendRequests.Add(request);

        var handler = new RespondToFriendRequestCommandHandler(
            friendRequests, users, new FakeUnitOfWork(), new FakeCurrentUserService(stranger.Id),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow), new FakeNotificationDispatcher());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new RespondToFriendRequestCommand(request.Id, FriendRequestStatus.Accepted), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyResponded_ThrowsConflictException()
    {
        var requester = MakeUser();
        var addressee = MakeUser();
        var users = new FakeUserRepository();
        users.Add(requester);
        users.Add(addressee);

        var friendRequests = new FakeFriendRequestRepository();
        var request = new FriendRequest
        {
            RequesterId = requester.Id, AddresseeId = addressee.Id,
            Status = FriendRequestStatus.Accepted, CreatedAt = DateTimeOffset.UtcNow, RespondedAt = DateTimeOffset.UtcNow
        };
        friendRequests.Add(request);

        var handler = new RespondToFriendRequestCommandHandler(
            friendRequests, users, new FakeUnitOfWork(), new FakeCurrentUserService(addressee.Id),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow), new FakeNotificationDispatcher());

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new RespondToFriendRequestCommand(request.Id, FriendRequestStatus.Accepted), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AddresseeAccepts_SetsAcceptedStatus()
    {
        var requester = MakeUser();
        var addressee = MakeUser();
        var users = new FakeUserRepository();
        users.Add(requester);
        users.Add(addressee);

        var friendRequests = new FakeFriendRequestRepository();
        var request = new FriendRequest
        {
            RequesterId = requester.Id, AddresseeId = addressee.Id,
            Status = FriendRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        };
        friendRequests.Add(request);

        var handler = new RespondToFriendRequestCommandHandler(
            friendRequests, users, new FakeUnitOfWork(), new FakeCurrentUserService(addressee.Id),
            new FakeDateTimeProvider(DateTimeOffset.UtcNow), new FakeNotificationDispatcher());

        var result = await handler.Handle(
            new RespondToFriendRequestCommand(request.Id, FriendRequestStatus.Accepted), CancellationToken.None);

        Assert.Equal(FriendRequestStatus.Accepted, result.Status);
        Assert.NotNull(request.RespondedAt);
    }
}
