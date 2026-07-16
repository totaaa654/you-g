using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Tests.Fakes;

public class FakePasswordResetTokenRepository : IPasswordResetTokenRepository
{
    public List<PasswordResetToken> Tokens { get; } = [];

    public Task<List<PasswordResetToken>> GetPendingForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(Tokens.Where(t => t.UserId == userId && t.UsedAt == null).ToList());

    public void Add(PasswordResetToken token) => Tokens.Add(token);
}
