using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for the real EF Core repository — lets Application-layer handlers be
/// unit-tested with no database, per the Repository pattern's stated purpose (docs/02-ARCHITECTURE.md Section 3).
/// </summary>
public class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = [];

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

    public Task<List<User>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken) =>
        Task.FromResult(Users.Where(u => ids.Contains(u.Id)).ToList());

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        Task.FromResult(Users.Any(u => u.Email == email));

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken) =>
        Task.FromResult(Users.Any(u => u.Username == username));

    public Task<bool> ExistsByFriendCodeAsync(string friendCode, CancellationToken cancellationToken) =>
        Task.FromResult(Users.Any(u => u.FriendCode == friendCode));

    public Task<(List<User> Users, int TotalCount)> SearchByUsernameAsync(
        string query, int page, int pageSize, CancellationToken cancellationToken)
    {
        var matches = Users
            .Where(u => !u.IsDeleted && u.Username.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(u => u.Username)
            .ToList();

        var pageOfMatches = matches.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult((pageOfMatches, matches.Count));
    }

    public void Add(User user) => Users.Add(user);
}
