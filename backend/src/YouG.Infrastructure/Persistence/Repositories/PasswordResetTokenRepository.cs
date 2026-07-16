using Microsoft.EntityFrameworkCore;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository(YouGDbContext dbContext) : IPasswordResetTokenRepository
{
    public Task<List<PasswordResetToken>> GetPendingForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.PasswordResetTokens.Where(t => t.UserId == userId && t.UsedAt == null).ToListAsync(cancellationToken);

    public void Add(PasswordResetToken token) => dbContext.PasswordResetTokens.Add(token);
}
