using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<User>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByFriendCodeAsync(string friendCode, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<bool> ExistsByFriendCodeAsync(string friendCode, CancellationToken cancellationToken);

    /// <summary>Username search, excluding soft-deleted accounts. Offset-paginated per the Phase 4 decision.</summary>
    Task<(List<User> Users, int TotalCount)> SearchByUsernameAsync(
        string query, int page, int pageSize, CancellationToken cancellationToken);

    void Add(User user);
}
