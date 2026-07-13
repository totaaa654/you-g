using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class UserRepository(YouGDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<List<User>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken) =>
        dbContext.Users.Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByFriendCodeAsync(string friendCode, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.FriendCode == friendCode, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public Task<bool> ExistsByFriendCodeAsync(string friendCode, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.FriendCode == friendCode, cancellationToken);

    public async Task<(List<User> Users, int TotalCount)> SearchByUsernameAsync(
        string query, int page, int pageSize, CancellationToken cancellationToken)
    {
        // Username is citext, so Contains() is already case-insensitive at the DB level.
        var matches = dbContext.Users.Where(u => !u.IsDeleted && u.IsSearchable && u.Username.Contains(query));

        var totalCount = await matches.CountAsync(cancellationToken);
        var users = await matches
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public void Add(User user) => dbContext.Users.Add(user);
}
