using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<User>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<bool> ExistsByFriendCodeAsync(string friendCode, CancellationToken cancellationToken);
    void Add(User user);
}
