using YouG.Application.Features.Profile.Queries.SearchUsers;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Features.Profile;

public class SearchUsersQueryHandlerTests
{
    private static User MakeUser(string username, bool isDeleted = false, bool isSearchable = true) => new()
    {
        Email = $"{username}@example.com",
        Username = username,
        FriendCode = $"YG-{username}",
        DisplayName = username,
        TimeZoneId = "UTC",
        IsDeleted = isDeleted,
        IsSearchable = isSearchable
    };

    [Fact]
    public async Task Handle_ExcludesSoftDeletedUsers()
    {
        var users = new FakeUserRepository();
        users.Users.Add(MakeUser("mayag"));
        users.Users.Add(MakeUser("mayadeleted", isDeleted: true));

        var handler = new SearchUsersQueryHandler(users);
        var result = await handler.Handle(new SearchUsersQuery("maya", 1, 20), CancellationToken.None);

        Assert.Single(result.Users);
        Assert.Equal("mayag", result.Users[0].Username);
    }

    [Fact]
    public async Task Handle_ExcludesNonSearchableUsers()
    {
        var users = new FakeUserRepository();
        users.Users.Add(MakeUser("mayag"));
        users.Users.Add(MakeUser("mayahidden", isSearchable: false));

        var handler = new SearchUsersQueryHandler(users);
        var result = await handler.Handle(new SearchUsersQuery("maya", 1, 20), CancellationToken.None);

        Assert.Single(result.Users);
        Assert.Equal("mayag", result.Users[0].Username);
    }

    [Fact]
    public async Task Handle_PaginatesResults()
    {
        var users = new FakeUserRepository();
        for (var i = 0; i < 5; i++)
        {
            users.Users.Add(MakeUser($"user{i}"));
        }

        var handler = new SearchUsersQueryHandler(users);
        var result = await handler.Handle(new SearchUsersQuery("user", 2, 2), CancellationToken.None);

        Assert.Equal(2, result.Users.Count);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Page);
    }
}
