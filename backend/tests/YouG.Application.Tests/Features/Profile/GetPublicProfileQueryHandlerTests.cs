using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Profile.Queries.GetPublicProfile;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Profile;

public class GetPublicProfileQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingUser_ReturnsPublicSubset()
    {
        var users = new FakeUserRepository();
        var user = new User
        {
            Email = "maya@example.com",
            Username = "mayag",
            FriendCode = "YG-ABC123",
            DisplayName = "Maya",
            Bio = "Loves board games",
            TimeZoneId = "UTC"
        };
        users.Users.Add(user);

        var handler = new GetPublicProfileQueryHandler(users);
        var result = await handler.Handle(new GetPublicProfileQuery(user.Id), CancellationToken.None);

        Assert.Equal("mayag", result.Username);
        Assert.Equal("Loves board games", result.Bio);
    }

    [Fact]
    public async Task Handle_UnknownUser_ThrowsNotFoundException()
    {
        var users = new FakeUserRepository();
        var handler = new GetPublicProfileQueryHandler(users);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetPublicProfileQuery(Guid.CreateVersion7()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SoftDeletedUser_ThrowsNotFoundException()
    {
        // A deleted account must look identical to a never-existing one from the outside.
        var users = new FakeUserRepository();
        var user = new User
        {
            Email = "deleted@example.com",
            Username = "deleted-user",
            FriendCode = "DEL0000000",
            DisplayName = "Deleted User",
            TimeZoneId = "UTC",
            IsDeleted = true
        };
        users.Users.Add(user);

        var handler = new GetPublicProfileQueryHandler(users);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetPublicProfileQuery(user.Id), CancellationToken.None));
    }
}
